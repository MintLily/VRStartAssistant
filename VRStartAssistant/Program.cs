using Serilog;
using VRStartAssistant.Secret;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.3";
    public const int TargetConfigVersion = 2;
}

public abstract class Program {
    public static AudioSwitch? AudioSwitchInstance;
    public static VRChat? VrChatInstance;
    public static VRCX? VrcxInstance;
    public static SteamVR? SteamVrInstance;
    private static WindowsXSO? _windowsXsoInstance;
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
        // var processes = new Processes();
        VrcxInstance = new VRCX();
        SteamVrInstance = new SteamVR();
        VrChatInstance = new VRChat();
        _windowsXsoInstance = new WindowsXSO();
        VrcVideoCacherInstance = new VRCVideoCacher();
        SecretApp1Instance = new SecretApp1();
        AdGoByeInstance = new AdGoBye();
        
        VrcxInstance.Start();                          // Start VRCX
#if DEBUG
        AudioSwitchInstance.Start().GetAwaiter().GetResult();
        Log.Debug("Press any key to exit...");
        Console.ReadLine();
        return Task.CompletedTask;
#else
        await SteamVrInstance.StartAsync();            // Start SteamVR, Start VRChat, Switch Audio
        WindowMinimizer.ShowWindow(WindowMinimizer.GetConsoleWindow(), 0); // Hide this console window
        await _windowsXsoInstance.StartAsync();
#endif
    }
}