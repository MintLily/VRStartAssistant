using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VRStartAssistant.Apps;
using VRStartAssistant.Configuration;
using VRStartAssistant.Features;
using VRStartAssistant.Features.Integrations;
using VRStartAssistant.Utils;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.16.0";
    public const int TargetConfigVersion = 12;
    internal static readonly string? BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build");
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
            MinimumLevel = LogEventLevel.Information // Vars.IsDebug ? LogEventLevel.Debug : LogEventLevel.Warning
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
        
        Log.Information("Base Directory: {0}", Vars.BaseDir);
        ConfigurationInstance = new Config();
        ConfigurationInstance.Load();

        // used for debugging audio switch code
        // AudioSwitch.Start(true);
        // ValidateAppPaths.Go();
        // Console.WriteLine("\n");
        // Log.Information("Configuration:\n{0}", ConfigurationInstance.ToJson());
        // Console.ReadLine();
        // return;

        await HASS.ToggleBaseStations(); // Turns on Base Stations
        await WindowsXSO.StartAsync(); // Start XSO & everything else
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");
    
    public static async Task StartApplications() {
        try {
            VRCX.Start(); // Start VRCX
            await VRChatOscRouter.Start(); // Start VRChat OSC Router
            await AdGoBye.Start(); // Start AdGoBye
            await Secret.SecretApp1.Start(); // Start SecretApp1
            await SteamVR.StartAsync(); // Start SteamVR, Start VRChat, Switch Audio, Custom Media OSC chatbox for VRChat
            await HOSCY.Start(); // Start HOSCY
            await HeartrateMonitor.Start(); // Start HeartRateOnStream-OSC
            await OSCLeash.Start(); // Start OSCLeash
            await Processes.GetOtherProcesses(); // Get Other Processes
            await WindowMinimizer.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye, HOSCY
        }
        catch (Exception ex) {
            Log.Error("Something in the Application Startup has failed: \n{0}", ex.Message + "\n" + ex.StackTrace);
        }
    }
    
    public static void CheckForExitedProcess() {
        if (Processes.SteamVrProcess is not { HasExited: true }) return;
        SteamVR.Exit().RunWithoutAwait();
        VRCVideoCacher.Exit();
        AdGoBye.Exit();
        VRCX.Exit();
        Secret.SecretApp1.Exit();
        HOSCY.Exit();
        HeartrateMonitor.Exit();
        OSCLeash.Exit();
        VRChatOscRouter.Exit();
        AudioSwitch.SwitchBack();
    }
}