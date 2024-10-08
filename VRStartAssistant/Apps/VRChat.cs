﻿using System.Diagnostics;
using Serilog;
using VRStartAssistant.Features;
using VRStartAssistant.Secret;

namespace VRStartAssistant.Apps; 

public class VRChat {
    public VRChat() => Logger.Information("Setting up module :: {Description}", "Starts and Minimizes VRChat");
    private static readonly ILogger Logger = Log.ForContext(typeof(VRChat));
    public static bool IsRunning;

    public static async Task Start() {
        if (IsRunning) return;
        Logger.Information("Starting VRChat...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/438100");
        IsRunning = true;
        await Task.Delay(TimeSpan.FromSeconds(5));
        AudioSwitch.Start();
        Logger.Information("Waiting 15 seconds for VRChat to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(15));
        
        Logger.Information("Attempting to detect VRChat...");
        Processes.VrChatProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrchat");
        if (Processes.VrChatProcess == null) {
            Logger.Warning("VRChat was {0}. Game will not minimize.", "not detected");
            return;
        }

        try {
            await VRCVideoCacher.Start();
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start VRCVideoCacher");
        }
    }
}