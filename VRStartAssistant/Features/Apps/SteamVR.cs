using System.Diagnostics;
using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Other;

namespace VRStartAssistant.Features.Apps;

public class SteamVR {
    public SteamVR() => Logger.Information("Setting up module :: {Description}", "Starts SteamVR");
    private static readonly ILogger Logger = Log.ForContext<SteamVR>();

    public static async Task StartAsync() {
        var config = Program.ConfigurationInstance!.Base!.VR;
        
        Logger.Information("Starting SteamVR...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/250820");
        try {
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
            if (Processes.SteamVrProcess != null) {
                Logger.Information("SteamVR is {0} with process ID {1}; not re-launching.", "already running", Processes.SteamVrProcess.Id);
                Logger.Information("SteamVR detected. This {0} will close when SteamVR closes...", Vars.AppName);
                return;
            }
        }
        catch { /*ignore*/ }
            
        Logger.Information("Waiting 5 seconds for SteamVR to fully start...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        
        if (config.AutoLaunchWithSteamVr) {
            if (config.HasRegistered is false) {
                var manifest = new vrappmanifest {
                    source = "builtin",
                    applications = [
                        new applications {
                            app_key = "dev.mintylabs.vrstartassistant",
                            app_type = "application",
                            launch_type = "binary",
                            binary_path_windows = "VRStartAssistant.exe",
                            is_dashboard_overlay = false,
                            strings = new langs {
                                en_us = new strings {
                                    name = "VRStartAssistant",
                                    description = "1"
                                }
                            }
                        }
                    ]
                };

                var steamVr = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "SteamVR");
                var manifestPath = Path.Combine(Environment.CurrentDirectory, "manifest.vrappmanifest");
                var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(manifestPath, json);
                
                await Task.Delay(TimeSpan.FromSeconds(1));
                try {
                    Process.Start(new ProcessStartInfo {
                        FileName = steamVr + "bin" + "win64" + "vrpathreg.exe",
                        Arguments = $"addapp {manifestPath}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                
                    config.HasRegistered = true;
                }
                catch {
                    Logger.Error("Failed to register {0} with SteamVR.", Vars.AppName);
                    return;
                }
            }
            else {
                Logger.Information("{0} is already registered with SteamVR.", Vars.AppName);
            }
        }
        
        try {
            Logger.Information("Attempting to detect SteamVR...");
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
            if (Processes.SteamVrProcess != null /* && Processes.SteamVrProcess.ProcessName.ToLower() == "vrserver"*/)
                Logger.Information("SteamVR detected. This {0} will close when SteamVR closes...", Vars.AppName);
            else
                Logger.Warning("SteamVR was {0}. Auto-Close with SteamVR is disabled. {1}", "not detected", "Please start this program after SteamVR has started.");
        }
        catch { /*ignore*/ }
    }

    public static void Exit() {
        if (Processes.SteamVrProcess == null) return;
        Logger.Information("SteamVR has exited.");
        OscMedia.StopMediaDetection();
    }
}