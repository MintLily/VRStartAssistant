using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VRStartAssistant.Configuration;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.15.0";
    public const int TargetConfigVersion = 11;
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

        await Integrations.HASS.ToggleBaseStations(); // Turns on Base Stations
        await WindowsXSO.StartAsync(); // Start XSO & everything else
    }

    public static void ChangeConsoleTitle(string extraData = "") => Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + (Vars.IsDebug ? " - DEBUG" : "") + (string.IsNullOrEmpty(extraData) ? "" : $" - {extraData}");
}