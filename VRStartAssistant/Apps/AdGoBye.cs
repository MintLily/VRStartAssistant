using System.Diagnostics;
using Serilog;

namespace VRStartAssistant.Apps;

public class AdGoBye {
    public AdGoBye() => Logger.Information("Setting up module :: {Description}", "Removes things from within VRChat");
    private static readonly ILogger Logger = Log.ForContext(typeof(AdGoBye));
    
    public async void Start() {
        try {
            Processes.AdGoBye = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "adgobye");
            if (Processes.AdGoBye != null) {
                Logger.Information("AdGoBye is {0} with process ID {1}; not re-launching.", "already running", Processes.AdGoBye.Id);
                return;
            }
        }
        catch {/*ignore*/}
        Logger.Information("Starting AdGoBye...");
        // Process.Start(Path.Combine(Vars.BaseDir, "extras", "AGB", "AdGoBye.exe"));
        Process.Start(new ProcessStartInfo {
            WorkingDirectory = Path.Combine(Vars.BaseDir, "extras", "AGB"),
            FileName = Path.Combine(Vars.BaseDir, "extras", "AGB", "AdGoBye.exe"),
            CreateNoWindow = false,
            UseShellExecute = false
        });
        await Task.Delay(TimeSpan.FromSeconds(1));
        Processes.AdGoBye = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName.ToLower() == "adgobye");
    }
    
    public void Exit() {
        if (Processes.AdGoBye == null) return;
        Logger.Information("Closing AdGoBye...");
        Processes.AdGoBye.CloseMainWindow();
        Processes.AdGoBye.Kill();
    }
}