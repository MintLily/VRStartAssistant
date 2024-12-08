using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Features.Apps; 

public class VRChat {
    public VRChat() => Logger.Information("Setting up module :: {Description}", "Starts and Minimizes VRChat");
    private static readonly ILogger Logger = Log.ForContext<VRChat>();

    public static async Task Start() {
        var didLoop = false;
        var looped = 0;
        Logger.Information("Starting VRChat...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/438100");
        
        loop:
        if (looped >= 3) {
            Logger.Warning("VRChat was {0} after {attempts} attempts. No longer trying again...", "not detected", looped);
            return;
        }
        
        Logger.Information($"Waiting {(didLoop ? 5 : 15)} seconds for VRChat to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(didLoop ? 5 : 15));
        
        Logger.Information("Attempting to detect VRChat...");
        Processes.VrChatProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrchat");
        if (Processes.VrChatProcess == null) {
            Logger.Warning("VRChat was {0}. Trying again.", "not detected");
            didLoop = true;
            looped++;
            goto loop;
        }
    }
}