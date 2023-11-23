using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class Processes {
    public Processes() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "Processes", "Process Module");
    
    public static Process? SteamVrProcess;
    public static Process? VrChatProcess;
    public static Process? VrcxProcess;
    public static Process? VrcVideoCacher;

    //public static void StartFirst() => SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrserver");
}