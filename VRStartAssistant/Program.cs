using Serilog;
using Serilog.Core;
using Serilog.Events;
using VRStartAssistant.Apps;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VRStartAssistant.Configuration;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.8.2";
    public const int TargetConfigVersion = 7;
    public static readonly string BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build");
}

public abstract class Program {
    public static Config? ConfigurationInstance;

    public static async Task Main(string[] args) {
        var levelSwitch = new LoggingLevelSwitch {
#if DEBUG
            MinimumLevel = LogEventLevel.Debug
#else
            MinimumLevel = LogEventLevel.Information
#endif
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
#if DEBUG
        await AudioSwitch.Start();
        Log.Debug("Press any key to exit...");
        Console.ReadKey();
        await Integrations.HASS.ToggleBaseStations(true);
        VRCX.Exit();
#else
        await Secret.SecretApp1.Start();         // Start SecretApp1
        await SteamVR.StartAsync();              // Start SteamVR, Start VRChat, Switch Audio
        await Processes.GetOtherProcesses();     // Get Other Processes
        await WindowMinimizer.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye
        await WindowsXSO.StartAsync();           // Start XSO
#endif
    }

    public static void ChangeConsoleTitle() {
#if DEBUG
        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + " - DEBUG";
#else
        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion;
#endif
    }
}