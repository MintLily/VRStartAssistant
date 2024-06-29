using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class HOSCY {
    public HOSCY() => Logger.Information("Setting up module :: {Description}", "Various OSC Things for VRChat");
    private static readonly ILogger Logger = Log.ForContext(typeof(HOSCY));
    public static bool IsRunning;
    
    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunHOSCY) return;
        try {
            Processes.HOSCY = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "HOSCY");
            if (Processes.HOSCY != null) {
                Logger.Information("HOSCY is {0} with process ID {1}; not re-launching.", "already running", Processes.HOSCY.Id);
                IsRunning = true;
                return;
            }
        }
        catch {/*ignore*/}

        try {
            Logger.Information("Starting HOSCY...");
            // Process.Start(Path.Combine(Vars.BaseDir, "extras", "HOSCY", "HOSCY.exe"));
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "HOSCY"),
                FileName = Path.Combine(Vars.BaseDir, "extras", "HOSCY VRChat Companion", "Hoscy.exe"),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            });
            await Task.Delay(TimeSpan.FromSeconds(1));
            Processes.HOSCY = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "HOSCY");
            IsRunning = true;
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start HOSCY");
        }
    }
    
    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunHOSCY) return;
        if (Processes.HOSCY == null) return;
        Logger.Information("Closing HOSCY...");
        Processes.HOSCY.CloseMainWindow();
        Processes.HOSCY.Kill();
    }
}