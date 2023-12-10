using Serilog;
using Serilog.Core;
using Serilog.Events;
using VRStartAssistant.Apps;
using VRStartAssistant.Secret;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.5";
    public const int TargetConfigVersion = 2;
    public static readonly string BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Visual Studio Projects", "VROnStartAssistant", "Build");
}

public abstract class Program {
    public static AudioSwitch? AudioSwitchInstance;
    private static WindowsXSO? _windowsXsoInstance;
    private static WindowMinimizer? _windowMinimizerInstance;
    
    public static VRChat? VrChatInstance;
    public static VRCX? VrcxInstance;
    public static SteamVR? SteamVrInstance;
    public static VRCVideoCacher? VrcVideoCacherInstance;
    public static SecretApp1? SecretApp1Instance;
    public static AdGoBye? AdGoByeInstance;

#if DEBUG
    public static Task Main(string[] args) {
#else
    public static async Task Main(string[] args) {
#endif
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
                template: "[{@t:HH:mm:ss} {@l:u3} {Coalesce(Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1),'<none>')}] {@m}\n{@x}",
                theme: TemplateTheme.Literate))
            .CreateLogger();
#if DEBUG
        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion + " - DEBUG";
#else
        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion;
#endif
        AudioSwitchInstance = new AudioSwitch();
        _windowsXsoInstance = new WindowsXSO();
        _windowMinimizerInstance = new WindowMinimizer();
        
        // var processes = new Processes();
        VrcxInstance = new VRCX();
        SteamVrInstance = new SteamVR();
        VrChatInstance = new VRChat();
        VrcVideoCacherInstance = new VRCVideoCacher();
        SecretApp1Instance = new SecretApp1();
        AdGoByeInstance = new AdGoBye();
        
        VrcxInstance.Start();                             // Start VRCX
#if DEBUG
        AudioSwitchInstance.Start().GetAwaiter().GetResult();
        Log.Debug("Press any key to exit...");
        Console.ReadLine();
        return Task.CompletedTask;
#else
        await SteamVrInstance.StartAsync();               // Start SteamVR, Start VRChat, Switch Audio
        await _windowMinimizerInstance.DelayedMinimize(); // Minimize VRChat, VRCVideoCacher, AdGoBye
        await _windowsXsoInstance.StartAsync();           // Start XSO
#endif
        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion;
    }
}