using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class SteamVR {
    public SteamVR() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "SteamVR", "Starts SteamVR");

    public async Task StartAsync() {
        Log.Information("Starting SteamVR...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/250820");

        Log.Information("Waiting 10 seconds for SteamVR to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(10));
        try {
            Log.Information("Attempting to detect SteamVR...");
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrserver");
            if (Processes.SteamVrProcess != null/* && Processes.SteamVrProcess.ProcessName.ToLower() == "vrserver"*/)
                Log.Information($"SteamVR detected. This {Vars.AppName} will close when SteamVR closes...");
            else
                Log.Warning("SteamVR was {0}. Auto-Close with SteamVR is disabled. {1}", "not detected", "Please start this program after SteamVR has started.");
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
        Log.Information("SteamVR has exited. Exiting in 5 seconds...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Process.GetCurrentProcess().Kill();
        Environment.Exit(0);
    }
    
}