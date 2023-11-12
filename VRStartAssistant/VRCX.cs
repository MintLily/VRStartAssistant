﻿using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRCX {
    public VRCX() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRCX", "Starts VRCX");

    public static void Start() {
        try {
            Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
            if (Processes.VrcxProcess != null) {
                Log.Information("VRCX is {0}; not re-launching.", "already running");
                return;
            }
        }
        catch {/*ignore*/}
        
        Log.Information("Starting VRCX...");
        Process.Start(new ProcessStartInfo {
            WorkingDirectory = @"G:\VRCX\",
            FileName = "VRCX.exe",
            Arguments = "",
            UseShellExecute = false,
        });
        Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
    }
    
    public static void AutoExitVrcxWithSteamVr() {
        if (Processes.VrcxProcess == null) return;
        Log.Information("Closing VRCX...");
        Processes.VrcxProcess.CloseMainWindow();
    }
}