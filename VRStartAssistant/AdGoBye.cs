using System.Diagnostics;
using Serilog;

namespace VRStartAssistant;

public class AdGoBye {
    public AdGoBye() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "AdGoBye", "Removes things from within VRChat");
    
    public void Start() {
        try {
            Processes.AdGoBye = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "adgobye");
            if (Processes.AdGoBye != null) {
                Log.Information("[{0}] AdGoBye is {1} with process ID {2}; not re-launching.", "ADGOBYE", "already running", Processes.AdGoBye.Id);
                return;
            }
        }
        catch {/*ignore*/}
        Log.Information("[{0}] Starting AdGoBye...", "ADGOBYE");
        Process.Start(Path.Combine(Environment.CurrentDirectory, "extras", "AGB", "AdGoBye.exe"));
        Processes.AdGoBye = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "adgobye");
    }
    
    public void AutoCloseWithVRChatOrSteamVR() {
        if (Processes.AdGoBye == null) return;
        Log.Information("[{0}] Closing AdGoBye...", "ADGOBYE");
        Processes.AdGoBye.CloseMainWindow();
    }
}