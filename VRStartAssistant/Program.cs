using Serilog;
using Serilog.Core;
using Serilog.Events;
using VRStartAssistant.Apps;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VRStartAssistant.Configuration;
using VRStartAssistant.Secret;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.9.0";
    public const int TargetConfigVersion = 8;
    public static readonly string BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build");
#if DEBUG
    public static bool IsDebug = true;
#else
    public static bool IsDebug;
#endif
}

public abstract class Program {
    public static Config? ConfigurationInstance;

    public static async Task Main(string[] args) {
        Vars.IsDebug = args.Contains("--debug");
        var levelSwitch = new LoggingLevelSwitch {
            MinimumLevel = Vars.IsDebug ? LogEventLevel.Debug : LogEventLevel.Warning
        };
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console(new ExpressionTemplate(
                template: "[{@t:HH:mm:ss} {@l:u3} {Coalesce(Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1),'VRSA')}] {@m}\n{@x}",
                theme: TemplateTheme.Literate))
            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "start_.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1024000000L)
            .CreateLogger();

        ChangeConsoleTitle();
        
        ConfigurationInstance = new Config();
        ConfigurationInstance.Load();
        
        await Integrations.HASS.ToggleBaseStations(); // Turns on Base Stations
        VRCX.Start();                                 // Start VRCX
        if (Vars.IsDebug) {
            await AudioSwitch.Start();
            Log.Debug("Press any key to exit...");
            Console.ReadKey();
            await Integrations.HASS.ToggleBaseStations(true);
            VRCX.Exit();
        }
        await AdGoBye.Start();                   // Start AdGoBye
        await SecretApp1.Start();                // Start SecretApp1
        await SteamVR.StartAsync();              // Start SteamVR, Start VRChat, Switch Audio, Custom Media OSC chatbox for VRChat
        await HOSCY.Start();                     // Start HOSCY
        await Processes.GetOtherProcesses();     // Get Other Processes
        await WindowMinimizer.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye, HOSCY
        await ConfigurationInstance.UpdateConfigEvery1Minute();
        await WindowsXSO.StartAsync();           // Start XSO
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");
}