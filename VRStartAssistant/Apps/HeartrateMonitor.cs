using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class HeartrateMonitor {
    public HeartrateMonitor() => Logger.Information("Setting up module :: {Description}", "Removes things from within VRChat");
    private static readonly ILogger Logger = Log.ForContext(typeof(HeartrateMonitor));
    public static bool IsRunning;
    
    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunHeartRateOnStream) return;
        try {
            Processes.HeartRateOnStream = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "HeartRateOnStream");
            if (Processes.HeartRateOnStream != null) {
                Logger.Information("HeartRateOnStream-OSC is {0} with process ID {1}; not re-launching.", "already running", Processes.HeartRateOnStream.Id);
                IsRunning = true;
                return;
            }
        }
        catch {/*ignore*/}

        try {
            Logger.Information("Starting HeartRateOnStream-OSC...");
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "AGB"),
                FileName = Path.Combine(Vars.BaseDir, "extras", "HROS", "HeartRateOnStream-OSC.exe"),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            });
            await Task.Delay(TimeSpan.FromSeconds(1));
            Processes.HeartRateOnStream = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "heartrateonstream-osc");
            IsRunning = true;
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start HeartRateOnStream-OSC");
        }
    }
    
    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunHeartRateOnStream) return;
        if (Processes.HeartRateOnStream == null) return;
        Logger.Information("Closing HeartRateOnStream-OSC...");
        Processes.HeartRateOnStream.CloseMainWindow();
        Processes.HeartRateOnStream.Kill();
    }
    
    public static void ValidatePath() {
        if (!Directory.Exists(Path.Combine(Vars.BaseDir, "extras", "HROS")))
            return;
        Logger.Information("HeartRateOnStream-OSC path is valid.");
    }
}