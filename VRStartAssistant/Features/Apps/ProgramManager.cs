using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Features.Apps;

public class ProgramManager {
    public ProgramManager() => Logger.Information("Setting up module :: {Description}", "Dynamically starts and stops programs based on the Configuration");
    private static readonly ILogger Logger = Log.ForContext<ProgramManager>();
    
    public static async Task StartApplications() {
        var config = Program.ConfigurationInstance!.Base;
        if (Vars.Verbose)
            Logger.Information("[{0}] Starting Applications...", "VERBOSE");

        foreach (var item in config!.Programs.Where(item => item.StartWithVrsa)) {
            if (string.IsNullOrWhiteSpace(item.Name))
                continue;
            if (item.HasMultiProcesses) {
                if (Vars.Verbose)
                    Logger.Information("[{0}] Checking for Pre-existing Multiple Processes", "VERBOSE");
                Processes.MultiApplications[item.Name!] = Process.GetProcesses().Where(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (Processes.MultiApplications[item.Name!]?.Count is not 0) {
                    List<int> ids = [];
                    Processes.MultiApplications[item.Name!]!.ForEach(x => ids.Add(x.Id));
                    Logger.Information("{Name} is {0} with {1} processes. [{2}]", item.Name, "already running", Processes.MultiApplications[item.Name!]!.Count, string.Join(", ", ids));
                    continue;
                }
            }
            else {
                if (Vars.Verbose)
                    Logger.Information("[{0}] Checking for Pre-existing Single Process", "VERBOSE");
                Processes.SingleApplications[item.Name!] = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
                if (Processes.SingleApplications[item.Name!] is not null) {
                    Logger.Information("{Name} is {0} with process ID {1}; not re-launching.", item.Name, "already running", Processes.SingleApplications[item.Name!]!.Id);
                    continue;
                }
            }
            
            Logger.Information("Starting {Name}...", item.Name);
            if (item.FallbackProcessStartingNeeded)
                Process.Start(item.ExePath!, item.Arguments!);
            else {
                try {
                    Process.Start(new ProcessStartInfo {
                        WorkingDirectory = Path.GetDirectoryName(item.ExePath),
                        FileName = item.ExePath,
                        Arguments = item.Arguments,
                        CreateNoWindow = false,
                        WindowStyle = item.StartMinimized ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex) {
                    Logger.Error(ex, "Failed to start {Name}", item.Name);
                }
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (item.HasMultiProcesses) {
                if (Vars.Verbose)
                    Logger.Information("[{0}] Finding Multiple Processes", "VERBOSE");
                Processes.MultiApplications[item.Name!] = Process.GetProcesses().Where(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (Processes.MultiApplications[item.Name!]?.Count is 0) continue;
                List<int> ids = [];
                Processes.MultiApplications[item.Name!]!.ForEach(x => ids.Add(x.Id));
                Logger.Information("{Name} has {0} with {1} processes. [{2}]", item.Name, "started", Processes.MultiApplications[item.Name!]!.Count, string.Join(", ", ids));
                continue;
            }
            if (Vars.Verbose)
                Logger.Information("[{0}] Finding Single Process", "VERBOSE");
            Processes.SingleApplications[item.Name!] = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Contains(item.ProcessName!, StringComparison.CurrentCultureIgnoreCase));
            if (Processes.SingleApplications[item.Name!] == null) continue;
            Logger.Information("{Name} has {0} with process ID {1}; not re-launching.", item.Name, "started", Processes.SingleApplications[item.Name!]!.Id);
        }
    }
    
    public static void ExitApplications() {
        var config = Program.ConfigurationInstance!.Base;

        foreach (var item in config!.Programs.Where(item => !string.IsNullOrWhiteSpace(item.Name))) {
            if (item.HasMultiProcesses) {
                Processes.MultiApplications.TryGetValue(item.Name!, out var applications);
                if (applications is null) continue;
                Logger.Information("Closing {Name}...", item.Name);
                foreach (var app in applications!) {
                    app.CloseMainWindow();
                    app.Kill();
                    if (Vars.Verbose)
                        Logger.Information("[{0}] Killed {1}", "VERBOSE", item.Name);
                }
                continue;
            }

            Processes.SingleApplications.TryGetValue(item.Name!, out var application);
            if (application is null) {
                Logger.Information("{Name} is null and was not running.", item.Name);
                continue;
            }
            Logger.Information("Closing {Name}...", item.Name);
            if (Vars.Verbose)
                Logger.Information("[{0}] ", "VERBOSE");
            application.CloseMainWindow();
            application.Kill();
            if (Vars.Verbose)
                Logger.Information("[{0}] Killed {1}", "VERBOSE", item.Name);
        }
    }
}