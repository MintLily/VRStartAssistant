using System.Diagnostics;
using Serilog.Events;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Features.Apps;

public class ProgramManager {
    public static async Task StartApplications() {
        var config = Program.ConfigurationInstance!.Base;

        foreach (var item in config!.Programs.Where(item => item.StartWithVrsa)) {
            if (string.IsNullOrWhiteSpace(item.Name))
                continue;
            if (item.HasMultiProcesses) {
                Processes.MultiApplications[item.Name!] = Process.GetProcesses().Where(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (Processes.MultiApplications[item.Name!]?.Count is not 0) {
                    List<int> ids = [];
                    Processes.MultiApplications[item.Name!]!.ForEach(x => ids.Add(x.Id));
                    continue;
                }
            }
            else {
                Processes.SingleApplications[item.Name!] = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
                if (Processes.SingleApplications[item.Name!] is not null)
                    continue;
            }
            
            if (item.FallbackProcessStartingNeeded)
                Process.Start(item.ExePath!, item.Arguments!);
            else {
                Try.Catch(() => {
                    Process.Start(new ProcessStartInfo {
                        WorkingDirectory = Path.GetDirectoryName(item.ExePath),
                        FileName = item.ExePath,
                        Arguments = item.Arguments,
                        CreateNoWindow = false,
                        WindowStyle = item.StartMinimized ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal,
                        UseShellExecute = true
                    });
                }, true);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (item.HasMultiProcesses) {
                Processes.MultiApplications[item.Name!] = Process.GetProcesses().Where(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (Processes.MultiApplications[item.Name!]?.Count is 0) continue;
                List<int> ids = [];
                Processes.MultiApplications[item.Name!]!.ForEach(x => ids.Add(x.Id));
                continue;
            }
            Processes.SingleApplications[item.Name!] = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
        }
    }
    
    public static void ExitApplications() {
        var config = Program.ConfigurationInstance!.Base;

        foreach (var item in config!.Programs.Where(item => !string.IsNullOrWhiteSpace(item.Name))) {
            if (item.HasMultiProcesses) {
                Processes.MultiApplications.TryGetValue(item.Name!, out var applications);
                if (applications is null)
                    continue;
                
                MainWindow.Instance.UpdateConsoleOutput($"[[[gold1]Program Manager[/]]] Closing [indianred_1]{item.Name}[/]...");
                foreach (var app in applications!) {
                    app.CloseMainWindow();
                    app.Kill();
                }
                
                continue;
            }

            Processes.SingleApplications.TryGetValue(item.Name!, out var application);
            if (application is null) {
                MainWindow.Instance.UpdateConsoleOutput($"[[[gold1]Program Manager[/]]] [indianred_1]{item.Name}[/] is null and was not running.", LogEventLevel.Error);
                continue;
            }
            
            MainWindow.Instance.UpdateConsoleOutput($"[[[gold1]Program Manager[/]]] Closing [indianred_1]{item.Name}[/]...");
            
            application.CloseMainWindow();
            application.Kill();
        }
    }

    public static async Task RelaunchAppIfCrashed() {
        // check windows process is it is running compared to the config
        var config = Program.ConfigurationInstance!.Base;
        
        var getProcesses = Process.GetProcesses().ToList();
        
        foreach (var item in config!.Programs.Where(item => item.RelaunchIfCrashed)) {
            if (string.IsNullOrWhiteSpace(item.Name))
                continue;
            
            var isRunning = getProcesses.FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
            if (isRunning is not null || !isRunning!.HasExited)
                continue;
            
            MainWindow.Instance.UpdateConsoleOutput($"[[[gold1]Program Manager[/]]] Relaunching [indianred_1]{item.Name}[/]...");

            if (item.HasMultiProcesses) {
                Processes.MultiApplications.TryGetValue(isRunning.ProcessName.ToLower(), out var applications);
                if (applications is null)
                    continue;
                Processes.MultiApplications.Remove(item.Name!);
            }
            else {
                Processes.SingleApplications.TryGetValue(isRunning.ProcessName.ToLower(), out var application);
                if (application is null)
                    continue;
                Processes.SingleApplications.Remove(item.Name!);
            }
            
            // relaunch the app
            if (item.FallbackProcessStartingNeeded)
                Process.Start(item.ExePath!, item.Arguments!);
            else {
                Try.Catch(() => {
                    Process.Start(new ProcessStartInfo {
                        WorkingDirectory = Path.GetDirectoryName(item.ExePath),
                        FileName = item.ExePath,
                        Arguments = item.Arguments,
                        CreateNoWindow = false,
                        WindowStyle = item.StartMinimized ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal,
                        UseShellExecute = true
                    });
                }, true);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (item.HasMultiProcesses) {
                Processes.MultiApplications[item.Name!] = Process.GetProcesses().Where(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (Processes.MultiApplications[item.Name!]?.Count is 0) continue;
                List<int> ids = [];
                Processes.MultiApplications[item.Name!]!.ForEach(x => ids.Add(x.Id));
                continue;
            }
            Processes.SingleApplications[item.Name!] = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}