using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class Processes {
    public Processes() => Logger.Information("Setting up module :: {Description}", "Process Module");
    private static readonly ILogger Logger = Log.ForContext(typeof(Processes));
    
    /* In-App Processes */
    public static Process? SteamVrProcess;
    public static Process? VrChatProcess;
    public static List<Process>? VrcxProcesses;
    public static Process? VrcVideoCacher;
    public static Process? AdGoBye;
    public static Process? BetterIndexFinger;

    /* External Processes */
    public static Process? WindowsTerminal;
    public static Process? Oyasumi;

    public async Task GetOtherProcesses() {
        await Task.Delay(TimeSpan.FromSeconds(8));
        Oyasumi = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "OyasumiVR");
        WindowsTerminal = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "WindowsTerminal");
    }
}