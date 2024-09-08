using System.Diagnostics;
using Serilog;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Apps; 

public class VRCX {
    public VRCX() => Logger.Information("Setting up module :: {Description}", "Starts VRCX");
    private static readonly ILogger Logger = Log.ForContext(typeof(VRCX));
    public static bool IsRunning;

    public static void Start() {
        if (IsRunning) return;
        try {
            Processes.VrcxProcesses = Process.GetProcesses().Where(p => p.ProcessName.Contains("vrcx", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (Processes.VrcxProcesses.Count != 0) {
                Logger.Information("VRCX is {0} with process ID {1}; not re-launching.", "already running", Processes.VrcxProcesses.First().Id);
                IsRunning = true;
                return;
            }
        }
        catch {/*ignore*/}
        
        Logger.Information("Starting VRCX...");
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VRCX", "VRCX.exe"));
        IsRunning = true;
        GetVrcxProcesses().RunWithoutAwait();
    }
    
    public static void Exit() {
        if (Processes.VrcxProcesses.Count == 0) return;
        Logger.Information("Closing VRCX...");
        foreach (var vrcx in Processes.VrcxProcesses) {
            vrcx.CloseMainWindow();
            vrcx.Kill();
        }
    }
    
    private static async Task GetVrcxProcesses() {
        await Task.Delay(TimeSpan.FromSeconds(30));
        Processes.VrcxProcesses = Process.GetProcesses().Where(p => p.ProcessName.Contains("vrcx", StringComparison.CurrentCultureIgnoreCase)).ToList();
        Logger.Debug("Got VRCX Processes: {0}", Processes.VrcxProcesses.Count);
    }
}