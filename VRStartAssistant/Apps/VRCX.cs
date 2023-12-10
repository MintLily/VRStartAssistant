using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps; 

public class VRCX {
    public VRCX() => Logger.Information("Setting up module :: {Description}", "Starts VRCX");
    private static readonly ILogger Logger = Log.ForContext(typeof(VRCX));

    public void Start() {
        try {
            Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
            if (Processes.VrcxProcess != null) {
                Logger.Information("VRCX is {0} with process ID {1}; not re-launching.", "already running", Processes.VrcxProcess.Id);
                return;
            }
        }
        catch {/*ignore*/}
        
        Logger.Information("Starting VRCX...");
        // Process.Start(new ProcessStartInfo {
        //     WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VRCX"),
        //     FileName = "VRCX.exe",
        //     Arguments = "",
        //     UseShellExecute = false,
        // });
        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VRCX", "VRCX.exe"));
        Processes.VrcxProcess = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "vrcx");
    }
    
    public void Exit() {
        if (Processes.VrcxProcess == null) return;
        Logger.Information("Closing VRCX...");
        Processes.VrcxProcess.CloseMainWindow();
        Processes.VrcxProcess.Kill();
    }
}