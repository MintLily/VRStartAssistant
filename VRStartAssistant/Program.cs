using Serilog;

namespace VRStartAssistant;

public static class Vars {
    public const string AppName = "VRStartAssistant";
    public const string WindowsTitle = "Automate VR Startup Things";
    public const string AppVersion = "1.0";
}

public abstract class Program {

    public static async Task Main(string[] args) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        Console.Title = Vars.WindowsTitle + " v" + Vars.AppVersion;
        
        VRCX.Start();                          // Start VRCX
        await SteamVR.StartAsync();            // Start SteamVR & VRChat
        var audioSwitcher = new AudioSwitch(); // Switch to VR USB Audio
        await WindowsXSO.StartAsync();         // Start custom WindowsXSO
        WindowMinimizer.ShowWindow(WindowMinimizer.GetConsoleWindow(), 0); // Hide this console window
    }
}