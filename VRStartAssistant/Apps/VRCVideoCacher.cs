﻿using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps; 

public class VRCVideoCacher {
    public VRCVideoCacher() => Logger.Information("Setting up module :: {Description}", "Starts VRCVideoCacher");
    private static readonly ILogger Logger = Log.ForContext(typeof(VRCVideoCacher));
    public static bool IsRunning;

    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunVrcVideoCacher) return;
        try {
            Processes.VrcVideoCacher = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrcvideocacher");
            IsRunning = true;
            if (Processes.VrcVideoCacher != null) {
                Logger.Information("VRCVideoCacher is {0} with process ID {1}; not re-launching.", "already running", Processes.VrcVideoCacher.Id);
                return;
            }
        }
        catch {/*ignore*/}
        Logger.Information("Starting VRCVideoCacher...");
        // Process.Start(Path.Combine(Vars.BaseDir, "extras", "VRCVideoCacher", "VRCVideoCacher.exe"));
        Process.Start(new ProcessStartInfo {
            WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "VRCVideoCacher"),
            FileName = Path.Combine(Vars.BaseDir, "extras", "VRCVideoCacher", "VRCVideoCacher.exe"),
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Minimized,
            UseShellExecute = true
        });
        await Task.Delay(TimeSpan.FromSeconds(1));
        Processes.VrcVideoCacher = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrcvideocacher");
        IsRunning = true;
    }
    
    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunVrcVideoCacher) return;
        if (Processes.VrcVideoCacher == null) return;
        Logger.Information("Closing VRCVideoCacher...");
        Processes.VrcVideoCacher.CloseMainWindow();
        Processes.VrcVideoCacher.Kill();
    }
    
    public static void ValidatePath() {
        if (!Directory.Exists(Path.Combine(Vars.BaseDir, "extras", "VRCVideoCacher")))
            return;
        Logger.Information("VRCVideoCacher path is valid.");
    }
}