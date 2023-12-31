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
    public const string AppVersion = "1.7.0";
    public const int TargetConfigVersion = 4;
    public static readonly string BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build");
}

public abstract class Program {
    public static Config? ConfigurationInstance;
    public static AudioSwitch? AudioSwitchInstance;
    private static WindowsXSO? _windowsXsoInstance;
    private static WindowMinimizer? _windowMinimizerInstance;
    private static Processes? _processesInstance;
    
    public static VRChat? VrChatInstance;
    public static VRCX? VrcxInstance;
    private static SteamVR? _steamVrInstance;
    public static VRCVideoCacher? VrcVideoCacherInstance;
    public static AdGoBye? AdGoByeInstance;

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
            .CreateLogger();

        ChangeConsoleTitle();
        
        ConfigurationInstance = new Config();
        ConfigurationInstance.Load();
        AudioSwitchInstance = new AudioSwitch();
        _windowsXsoInstance = new WindowsXSO();
        _windowMinimizerInstance = new WindowMinimizer();
        _processesInstance = new Processes();
        
        // var processes = new Processes();
        VrcxInstance = new VRCX();
        _steamVrInstance = new SteamVR();
        VrChatInstance = new VRChat();
        VrcVideoCacherInstance = new VRCVideoCacher();
        AdGoByeInstance = new AdGoBye();
        
        await Integrations.HASS.ToggleBaseStations();     // Turns on Base Stations
        VRCX.Start();                                     // Start VRCX
#if DEBUG
        AudioSwitchInstance.Start().GetAwaiter().GetResult();
        Log.Debug("Press any key to exit...");
        Console.ReadLine();
        await Integrations.HASS.ToggleBaseStations(true);
#else
        await _steamVrInstance.StartAsync();              // Start SteamVR, Start VRChat, Switch Audio
        await _processesInstance.GetOtherProcesses();     // Get Other Processes
        await _windowMinimizerInstance.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye
        await _windowsXsoInstance.StartAsync();           // Start XSO
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