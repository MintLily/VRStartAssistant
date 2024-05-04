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
    public const string AppVersion = "1.10.1";
    public const int TargetConfigVersion = 9;
    internal static string? BaseDir;
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
            MinimumLevel = LogEventLevel.Information// Vars.IsDebug ? LogEventLevel.Debug : LogEventLevel.Warning
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
        Vars.BaseDir = ConfigurationInstance.Base.SetBaseDirectoryToDev
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "VRSA");
        Log.Information("Base Directory: {0}", Vars.BaseDir);
        
        await Integrations.HASS.ToggleBaseStations(); // Turns on Base Stations
        VRCX.Start();                                 // Start VRCX
        if (Vars.IsDebug) {
            await AudioSwitch.Start();
            Log.Debug("Press any key to exit...");
            Console.ReadKey();
            await Integrations.HASS.ToggleBaseStations(true);
            VRCX.Exit();
        }

        try {
            await AdGoBye.Start(); // Start AdGoBye
            await SecretApp1.Start(); // Start SecretApp1
            await SteamVR.StartAsync(); // Start SteamVR, Start VRChat, Switch Audio, Custom Media OSC chatbox for VRChat
            await HOSCY.Start(); // Start HOSCY
            await Processes.GetOtherProcesses(); // Get Other Processes
            await WindowMinimizer.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye, HOSCY
            // await ConfigurationInstance.UpdateConfigEvery1Minute();
            await WindowsXSO.StartAsync(); // Start XSO
            // WindowsXSO.NotificationThread.Start();   // Start XSO
        }
        catch (Exception ex) {
            Log.Error("Something in the Startup has failed: \n{0}", ex.Message);
        }
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");
}