using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRChat {
    public VRChat() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRChat", "Starts and Minimizes VRChat");

    public async Task Start() {
        Log.Information("[{0}] Starting VRChat...", "VRCHAT");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/438100");
        await Task.Delay(TimeSpan.FromSeconds(5));
        await Program.AudioSwitchInstance.Start();
        Log.Information("[{0}] Waiting 20 seconds for VRChat to fully start...", "VRCHAT");
        await Task.Delay(TimeSpan.FromSeconds(20));
        
        Log.Information("[{0}] Attempting to detect VRChat...", "VRCHAT");
        Processes.VrChatProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrchat");
        if (Processes.VrChatProcess == null) {
            Log.Warning("[{0}] VRChat was {1}. Game will not minimize.", "VRCHAT", "not detected");
            return;
        }
        Log.Information("[{0}] VRChat detected. Minimizing VRChat...", "VRCHAT");
        WindowMinimizer.ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);

        try {
            await Program.VrcVideoCacherInstance.Start();
        }
        catch (Exception ex) {
            Log.Error(ex, "[{0}] {1}", "VRCHAT", "Failed to start VRCVideoCacher");
            Program.VrcVideoCacherInstance.FailedToStart = true;
        }
        await Program.SecretApp1Instance.Start();
        Program.AdGoByeInstance.Start();
    }
}