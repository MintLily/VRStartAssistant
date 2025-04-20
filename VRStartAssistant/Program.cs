using System.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using VRStartAssistant.Configuration;
using VRStartAssistant.Features;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Spectre.Console;
#if DEBUG
using VRStartAssistant.Configuration.debug;
#endif
using VRStartAssistant.Features.Apps;
using Console = System.Console;
using HASS = VRStartAssistant.Features.Integrations.HASS;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "2.1.0";
    public const int TargetConfigVersion = 15;
#if DEBUG
    public const bool IsDebug = true;
#else
    public const bool IsDebug = false;
#endif
    public static bool LaunchOption_PrintAudioDevices { get; set; }
    public static bool LaunchOption_GetAudioDevices { get; set; }
}

public abstract class Program {
    internal static Config? ConfigurationInstance;
#if DEBUG
    internal static debugconfig? DebugConfigurationInstance;
#endif
    // private static readonly byte[] MintGreen = [ 159, 255, 227 ];
    internal static ILogger MainLogger;
    private static MainWindow MainWindow;

    public static async Task Main(string[]? args = null) {
        if (args is not null && args.Length > 0) {
            foreach (var arg in args) {
                if (arg == "--print-audio-devices") {
                    Vars.LaunchOption_PrintAudioDevices = true;
                }
                else if (arg == "--get-audio-devices") {
                    Vars.LaunchOption_GetAudioDevices = true;
                }
            }
        }
        
        ChangeConsoleTitle();
#if !DEBUG
        await new Updater.Updater().Start();
#endif
        
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
        MainLogger = Log.Logger;
        
#if DEBUG
        DebugConfigurationInstance = new debugconfig();
        DebugConfigurationInstance.Load();
#endif
        ConfigurationInstance = new Config();
        ConfigurationInstance.Load();

        MainWindow = new MainWindow();
        // await MainWindow.StartingGui();
        await MainWindow.Gui();
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.AppName + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");

    public static async Task StartApplications() {
        try {
            await ProgramManager.StartApplications();
            
            await SteamVR.StartAsync(); // Start SteamVR and / or register a vrappmanifest
            MainWindow.Instance.UpdateConsoleOutput("[[[gold1]Program Manager[/]]] SteamVR has [darkseagreen3_1]started[/].");
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            AudioSwitch.Start();
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            await VRChat.Start();
            MainWindow.Instance.UpdateConsoleOutput("[[[gold1]Program Manager[/]]] VRChat has [darkseagreen3_1]started[/].");
            
            await Task.Delay(TimeSpan.FromSeconds(30));
            await WindowMinimizer.Minimize();
        }
        catch (Exception ex) {
            Console.Clear();
            AnsiConsole.WriteLine($"Something in the Application Startup has failed: {ex.Message} \n {ex.StackTrace}");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public static async Task CheckForExitedProcess() {
        await Task.Delay(TimeSpan.FromSeconds(2));
        AudioSwitch.SwitchBack();
        await HASS.ToggleBaseStations(true);
        ProgramManager.ExitApplications();
        SteamVR.Exit();
        Process.GetCurrentProcess().Kill();
        Environment.Exit(0);
    }
}