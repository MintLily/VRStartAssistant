using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class VRChat {
    public VRChat() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "VRChat", "Starts and Minimizes VRChat");

    public static async Task Start() {
        Log.Information("Starting VRChat...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/438100");
        Log.Information("Waiting 20 seconds for VRChat to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(20));
        
        Log.Information("Attempting to detect VRChat...");
        Processes.VrChatProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrchat");
        if (Processes.VrChatProcess == null) {
            Log.Warning("VRChat was {0}. Game will not minimize.", "not detected");
            return;
        }
        Log.Information("VRChat detected. Minimizing VRChat...");
        WindowMinimizer.ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);
        
        await Program.AudioSwitchInstance.Start();
    }
}