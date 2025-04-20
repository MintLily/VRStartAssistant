using System.Diagnostics;
using System.Text.Json;
using VRStartAssistant.Configuration.Other;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Features.Apps;

public class SteamVR {
    public static async Task StartAsync() {
        var config = Program.ConfigurationInstance!.Base!.VR;
        
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steam.exe"), "steam://rungameid/250820");
        
        var isNull = false;
        Try.Catch(() => {
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
            isNull = Processes.SteamVrProcess != null;
        }, true);

        if (isNull)
            return;
        
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
                Try.Catch(() => {
                    Process.Start(new ProcessStartInfo {
                        FileName = steamVr + "bin" + "win64" + "vrpathreg.exe",
                        Arguments = $"addapp {manifestPath}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                
                    config.HasRegistered = true;
                }, true);
            }
        }

        Try.Catch(() => {
            Processes.SteamVrProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vrserver");
        }, true);
    }

    public static void Exit() {
        if (Processes.SteamVrProcess == null) return;
        MainWindow.Instance.UpdateConsoleOutput("SteamVR has exited.");
        OscMedia.StopMediaDetection();
    }
}