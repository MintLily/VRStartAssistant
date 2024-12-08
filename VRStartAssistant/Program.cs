using System.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VRStartAssistant.Configuration;
using VRStartAssistant.Features;
using VRStartAssistant.Features.Integrations;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using VRStartAssistant.Features.Apps;
using VRStartAssistant.Updater;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "2.0.0";
    public const int TargetConfigVersion = 14;
#if DEBUG
    public static bool IsDebug = true;
    public static bool Verbose = true;
#else
    public static bool IsDebug { get; set; }
    public static bool Verbose { get; set; }
#endif
    public static bool GetAudioDevices { get; set; }
}

public abstract class Program {
    internal static Config? ConfigurationInstance;

    public static async Task Main(string[] args) {
        var root1 = new RootCommand { new Option<bool>("--debug", "Switch operations to run certain code but not others") };
        var root2 = new RootCommand { new Option<bool>("--verbose", "Print everything to the console") };
        var root3 = new RootCommand { new Option<bool>("--get-audio-devices", "Print all audio devices to the console") };
        
        root1.Handler = CommandHandler.Create<bool>(x => Vars.IsDebug = x);
        root2.Handler = CommandHandler.Create<bool>(x => Vars.Verbose = x);
        root3.Handler = CommandHandler.Create<bool>(x => Vars.GetAudioDevices = x);

        await root1.InvokeAsync(args);
        await root2.InvokeAsync(args);
        await root3.InvokeAsync(args);
        
        var levelSwitch = new LoggingLevelSwitch {
            MinimumLevel = LogEventLevel.Information // Vars.IsDebug ? LogEventLevel.Debug : LogEventLevel.Warning
        };
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console(new ExpressionTemplate(
                template: "[{@t:HH:mm:ss} {@l:u3} {Coalesce(Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1),'VRSA')}] {@m}\n{@x}",
                theme: TemplateTheme.Literate))
            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "start_.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 5,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 5000000L) // Split file if over 5MB
            .CreateLogger();

        ChangeConsoleTitle();
        await new Updater.Updater().Start();
        
        Log.Information("Base Directory: {0}", AppDomain.CurrentDomain.BaseDirectory);
        ConfigurationInstance = new Config();
        ConfigurationInstance.Load();
        
        if (Vars.GetAudioDevices)
            AudioSwitch.GetAudioDevices();
        
        if (Vars.IsDebug) {
            AudioSwitch.Start(Vars.IsDebug);
            Log.Information("Configuration:\n{0}", ConfigurationInstance.ToJson());
            await ProgramManager.StartApplications();
            Console.ReadLine();
            ProgramManager.ExitApplications();
            Console.ReadLine();
            return;
        }

        await HASS.ToggleBaseStations(); // Turns on Base Stations
        
        ChangeConsoleTitle();
        await StartApplications();
        while (true) {
            // Check if SteamVR is still running, if so, kill other applications
            if (Processes.SteamVrProcess is not { HasExited: true }) continue;
            await CheckForExitedProcess();
            break;
        }
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");

    private static async Task StartApplications() {
        try {
            await ProgramManager.StartApplications();
            await SteamVR.StartAsync(); // Start SteamVR and / or register a vrappmanifest
            await Task.Delay(TimeSpan.FromSeconds(5));
            AudioSwitch.Start();
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            await VRChat.Start();
            
            await Task.Delay(TimeSpan.FromSeconds(30));
            await WindowMinimizer.Minimize();
        }
        catch (Exception ex) {
            Log.Error("Something in the Application Startup has failed: \n{0}", ex.Message + "\n" + ex.StackTrace);
        }
    }

    private static async Task CheckForExitedProcess() {
        Log.Warning("SteamVR has exited. Closing all other processes...");
        await Task.Delay(TimeSpan.FromSeconds(2));
        AudioSwitch.SwitchBack();
        await HASS.ToggleBaseStations(true);
        ProgramManager.ExitApplications();
        SteamVR.Exit();
        Process.GetCurrentProcess().Kill();
        Environment.Exit(0);
    }
}