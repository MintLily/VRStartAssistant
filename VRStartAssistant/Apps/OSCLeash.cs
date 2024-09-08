using System.Diagnostics;
using Serilog;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Apps;

public class OSCLeash {
    public OSCLeash() => Logger.Information("Setting up module :: {Description}", "Space Drag via OSC");
    private static readonly ILogger Logger = Log.ForContext(typeof(OSCLeash));
    public static bool IsRunning;

    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunOscLeash) return;
        try {
            Processes.OSCLeash = Process.GetProcesses().Where(p => p.ProcessName.Contains("OSCLeash", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (Processes.OSCLeash.Count != 0) {
                Logger.Information("OSCLeash is {0} with process ID {1}; not re-launching.", "already running", Processes.OSCLeash.First().Id);
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
            GetOscLeashProcesses().RunWithoutAwait();
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start OSCLeash");
        }
    }
    
    private static async Task GetOscLeashProcesses() {
        await Task.Delay(TimeSpan.FromSeconds(30));
        Processes.OSCLeash = Process.GetProcesses().Where(p => p.ProcessName.Contains("OSCLeash", StringComparison.CurrentCultureIgnoreCase)).ToList();
        Logger.Debug("Got OSCLeash Processes: {0}", Processes.OSCLeash.Count);
    }
    
    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunOscLeash) return;
        if (Processes.OSCLeash.Count == 0) return;
        Logger.Information("Closing OSCLeash...");
        foreach (var oscLeash in Processes.OSCLeash) {
            oscLeash.CloseMainWindow();
            oscLeash.Kill();
        }
    }
    
    public static void ValidatePath() {
        if (!Directory.Exists(Path.Combine(Vars.BaseDir, "extras", "OSCLeash")))
            return;
        Logger.Information("OSCLeash path is valid.");
    }
}