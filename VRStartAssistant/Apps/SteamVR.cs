﻿using System.Diagnostics;
using Serilog;
using VRStartAssistant.Features;
using VRStartAssistant.Features.Integrations;

namespace VRStartAssistant.Apps; 

public class SteamVR {
    public SteamVR() => Logger.Information("Setting up module :: {Description}", "Starts SteamVR");
    private static readonly ILogger Logger = Log.ForContext(typeof(SteamVR));
    public static bool IsRunning;

    public static async Task StartAsync() {
        if (IsRunning) return;
        Logger.Information("Starting SteamVR...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/250820");
        try {
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
            IsRunning = true;
            if (Processes.SteamVrProcess != null) {
                Logger.Information("SteamVR is {0} with process ID {1}; not re-launching.", "already running", Processes.SteamVrProcess.Id);
                Logger.Information("SteamVR detected. This {0} will close when SteamVR closes...", Vars.AppName);
                await Task.Delay(TimeSpan.FromSeconds(2));
                await VRChat.Start();
                return;
            }
        }
        catch {/*ignore*/}

        Logger.Information("Waiting 5 seconds for SteamVR to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        try {
            Logger.Information("Attempting to detect SteamVR...");
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
            IsRunning = true;
            if (Processes.SteamVrProcess != null/* && Processes.SteamVrProcess.ProcessName.ToLower() == "vrserver"*/)
                Logger.Information("SteamVR detected. This {0} will close when SteamVR closes...", Vars.AppName);
            else
                Logger.Warning("SteamVR was {0}. Auto-Close with SteamVR is disabled. {1}", "not detected", "Please start this program after SteamVR has started.");
        }
        catch {
            // ignored
        }

        await Task.Delay(TimeSpan.FromSeconds(2));
        await VRChat.Start();
    }
    
    public static async Task Exit() {
        if (Processes.SteamVrProcess == null) return;
        Logger.Information("SteamVR has exited."/*" Exiting in 2 seconds..."*/);
        // await Task.Delay(TimeSpan.FromSeconds(2));
        // if (WindowsXSO.NotificationThread.IsAlive)
        //     WindowsXSO.NotificationThread.Abort();
        // WindowsXSO.run = false;
        OscMedia.StopMediaDetection();
        await HASS.ToggleBaseStations(true);
        Process.GetCurrentProcess().Kill();
        Environment.Exit(0);
    }
    
}