using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class BetterIndexFinger {
    public BetterIndexFinger() => Logger.Information("Setting up module :: {Description}", "Starts BetterIndexFinger");
    private static readonly ILogger Logger = Log.ForContext(typeof(BetterIndexFinger));

    public static void Start() {
        if (!Program.ConfigurationInstance.Base.RunBetterIndexFinger) return;
        try {
            Processes.BetterIndexFinger = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "BetterIndexFinger");
            if (Processes.BetterIndexFinger != null) {
                Logger.Information("BetterIndexFinger is {0} with process ID {1}; not re-launching.", "already running", Processes.BetterIndexFinger.Id);
                return;
            }
        }
        catch {/*ignore*/}
        Logger.Information("Starting VRCX...");
        Process.Start(new ProcessStartInfo {
            WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "BetterIndexFinger", "BetterIndexFinger"),
            FileName = Path.Combine(Vars.BaseDir, "extras", "BetterIndexFinger", "BetterIndexFinger", "BetterIndexFinger.exe"),
            CreateNoWindow = false,
            UseShellExecute = false
        });
        Processes.BetterIndexFinger = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "BetterIndexFinger");
    }
}