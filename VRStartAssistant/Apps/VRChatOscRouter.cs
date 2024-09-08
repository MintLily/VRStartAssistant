using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class VRChatOscRouter {
    public VRChatOscRouter() => Logger.Information("Setting up module :: {Description}", "Port router manager for VRChat");
    private static readonly ILogger Logger = Log.ForContext(typeof(VRChatOscRouter));
    public static bool IsRunning;

    public static async Task Start() {
        if (IsRunning) return;
        if (!Program.ConfigurationInstance.Base.RunVRChatOSCRouter) return;
        try {
            Processes.Vor = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vor");
            if (Processes.Vor != null) {
                Logger.Information("VRChat OSC Router is {0} with process ID {1}; not re-launching.", "already running", Processes.Vor.Id);
                IsRunning = true;
                return;
            }
        }
        catch { /*ignore*/
        }

        try {
            Logger.Information("Starting VRChat OSC Router...");
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "vor", "bin"),
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "vor", "bin", "vor.exe"),
                Arguments = "--enable-on-start / -e",
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            });
            await Task.Delay(TimeSpan.FromSeconds(1));
            Processes.Vor = Process.GetProcesses().ToList().FirstOrDefault(p => p?.ProcessName.ToLower() == "vor");
            IsRunning = true;
        }
        catch (Exception ex) {
            Logger.Error(ex, "Failed to start VRChat OSC Router");
        }
    }

    public static void Exit() {
        if (!Program.ConfigurationInstance.Base.RunVRChatOSCRouter) return;
        if (Processes.Vor == null) return;
        Logger.Information("Closing VRChat OSC Router...");
        Processes.Vor.CloseMainWindow();
        Processes.Vor.Kill();
    }
    
    public static void ValidatePath() {
        if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "vor", "bin")))
            return;
        Logger.Information("VRChat OSC Router path is valid.");
    }
}