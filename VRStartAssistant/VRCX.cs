using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRCX {
    public VRCX() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRCX", "Starts VRCX");

    public void Start() {
        try {
            Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
            if (Processes.VrcxProcess != null) {
                Log.Information("[{0}] VRCX is {1} with process ID {2}; not re-launching.", "VRCX", "already running", Processes.VrcxProcess.Id);
                return;
            }
        }
        catch {/*ignore*/}
        
        Log.Information("[{0}] Starting VRCX...", "VRCX");
        // Process.Start(new ProcessStartInfo {
        //     WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VRCX"),
        //     FileName = "VRCX.exe",
        //     Arguments = "",
        //     UseShellExecute = false,
        // });
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VRCX", "VRCX.exe"));
        Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
    }
    
    public void AutoExitVrcxWithSteamVr() {
        if (Processes.VrcxProcess == null) return;
        Log.Information("[{0}] Closing VRCX...", "VRCX");
        Processes.VrcxProcess.CloseMainWindow();
    }
}