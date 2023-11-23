using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class SteamVR {
    public SteamVR() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "SteamVR", "Starts SteamVR");

    public async Task StartAsync() {
        Log.Information("[{0}] Starting SteamVR...", "STEAMVR");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/250820");

        Log.Information("[{0}] Waiting 10 seconds for SteamVR to fully start...", "STEAMVR");
        await Task.Delay(TimeSpan.FromSeconds(10));
        try {
            Log.Information("[{0}] Attempting to detect SteamVR...", "STEAMVR");
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrserver");
            if (Processes.SteamVrProcess != null/* && Processes.SteamVrProcess.ProcessName.ToLower() == "vrserver"*/)
                Log.Information("[{0}] SteamVR detected. This {1} will close when SteamVR closes...", "STEAMVR", Vars.AppName);
            else
                Log.Warning("[{0}] SteamVR was {1}. Auto-Close with SteamVR is disabled. {2}", "STEAMVR", "not detected", "Please start this program after SteamVR has started.");
        }
        catch {
            // ignored
        }

        await Task.Delay(TimeSpan.FromSeconds(2));
        await Program.VrChatInstance.Start();
    }
    
    public async Task ExitApplicationWithSteamVr() {
        Program.VrcxInstance.AutoExitVrcxWithSteamVr();
        if (Processes.SteamVrProcess == null) return;
        Log.Information("[{0}] SteamVR has exited. Exiting in 5 seconds...", "STEAMVR");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Process.GetCurrentProcess().Kill();
        Environment.Exit(0);
    }
    
}