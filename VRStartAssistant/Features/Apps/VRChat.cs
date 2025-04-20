using System.Diagnostics;

namespace VRStartAssistant.Features.Apps; 

public class VRChat {
    public static async Task Start() {
        var didLoop = false;
        var looped = 0;
        
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/438100");
        
        loop:
        if (looped >= 3)
            return;
        
        await Task.Delay(TimeSpan.FromSeconds(didLoop ? 5 : 15));
        
        Processes.VrChatProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrchat");
        if (Processes.VrChatProcess == null) {
            didLoop = true;
            looped++;
            goto loop;
        }
    }
}