using Serilog;
using VRStartAssistant.Secret;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.2.1";
}

public abstract class Program {
    public static AudioSwitch? AudioSwitchInstance;
    public static VRChat? VrChatInstance;
    public static VRCX? VrcxInstance;
    public static SteamVR? SteamVrInstance;
    private static WindowsXSO? _windowsXsoInstance;
    public static VRCVideoCacher? VrcVideoCacherInstance;
    public static SecretApp1? SecretApp1Instance;

    public static async Task Main(string[] args) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion;
        AudioSwitchInstance = new AudioSwitch();
        var processes = new Processes();
        VrcxInstance = new VRCX();
        SteamVrInstance = new SteamVR();
        VrChatInstance = new VRChat();
        _windowsXsoInstance = new WindowsXSO();
        VrcVideoCacherInstance = new VRCVideoCacher();
        SecretApp1Instance = new SecretApp1();
        
        VrcxInstance.Start();                          // Start VRCX
        await SteamVrInstance.StartAsync();            // Start SteamVR, Start VRChat, Switch Audio
        WindowMinimizer.ShowWindow(WindowMinimizer.GetConsoleWindow(), 0); // Hide this console window
        await _windowsXsoInstance.StartAsync();
    }
}