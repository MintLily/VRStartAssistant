using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class OSCLeash {
    public OSCLeash() => Logger.Information("Setting up module :: {Description}", "Space Drag via OSC");
    private static readonly ILogger Logger = Log.ForContext(typeof(OSCLeash));
    public static bool IsRunning;

    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunOscLeash) return;
        try {
            Processes.OSCLeash = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "OSCLeash");
            if (Processes.OSCLeash != null) {
                Logger.Information("OSCLeash is {0} with process ID {1}; not re-launching.", "already running", Processes.OSCLeash.Id);
                IsRunning = true;
                return;
            }
        }
        catch {/*ignore*/}

        try {
            Logger.Information("Starting OSCLeash...");
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "OSCLeash"),
                FileName = Path.Combine(Vars.BaseDir, "extras", "OSCLeash", "OSCLeash.exe"),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            });
            await Task.Delay(TimeSpan.FromSeconds(1));
            Processes.OSCLeash = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "OSCLeash");
            IsRunning = true;
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start OSCLeash");
        }
    }
    
    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunOscLeash) return;
        if (Processes.OSCLeash == null) return;
        Logger.Information("Closing OSCLeash...");
        Processes.OSCLeash.CloseMainWindow();
        Processes.OSCLeash.Kill();
    }
    
    public static void ValidatePath() {
        if (!Directory.Exists(Path.Combine(Vars.BaseDir, "extras", "OSCLeash")))
            return;
        Logger.Information("OSCLeash path is valid.");
    }
}