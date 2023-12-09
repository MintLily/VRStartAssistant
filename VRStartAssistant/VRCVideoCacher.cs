using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRCVideoCacher {
    public VRCVideoCacher() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRCVideoCacher", "Starts VRCVideoCacher");
    public bool FailedToStart;

    public async Task Start() {
        try {
            Processes.VrcVideoCacher = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcvideocacher");
            if (Processes.VrcVideoCacher != null) {
                Log.Information("[{0}] VRCVideoCacher is {1} with process ID {2}; not re-launching.", "VRCVIDEOCACHER", "already running", Processes.VrcVideoCacher.Id);
                return;
            }
        }
        catch {/*ignore*/}
        Log.Information("[{0}] Starting VRCVideoCacher...", "VRCVIDEOCACHER");
        Process.Start(Path.Combine(Environment.CurrentDirectory, "extras", "VRCVideoCacher", "VRCVideoCacher.exe"));
        Processes.VrcVideoCacher = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcvideocacher");
    }
    
    public void AutoCloseWithVRChatOrSteamVR() {
        if (Processes.VrcVideoCacher == null) return;
        Log.Information("[{0}] Closing VRCVideoCacher...", "VRCVIDEOCACHER");
        Processes.VrcVideoCacher.CloseMainWindow();
    }
}