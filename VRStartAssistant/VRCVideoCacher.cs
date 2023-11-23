using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRCVideoCacher {
    public VRCVideoCacher() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRCVideoCacher", "Starts VRCVideoCacher");

    public async Task Start() {
        Log.Information("[{0}] Starting VRCVideoCacher...", "VRCVIDEOCACHER");
        Process.Start(Path.Combine(Environment.CurrentDirectory, "extras", "VRCVideoCacher", "VRCVideoCacher.exe"));
        Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcvideocacher");
    }
}