using System.Runtime.InteropServices;
using Serilog;

namespace VRStartAssistant.Features; 

public class WindowMinimizer {
    public WindowMinimizer() => Logger.Information("Setting up module :: {Description}", "Functions to minimize windows");
    
    private static readonly ILogger Logger = Log.ForContext<WindowMinimizer>();
    
    // [DllImport("user32.dll", EntryPoint = "FindWindow")]
    // private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();
    
    public static async Task Minimize() {
        ShowWindow(GetConsoleWindow(), 0); // Hide this console window

        if (Processes.VrChatProcess is not null) {
            Logger.Information("Minimizing VRChat...");
            ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);
        }

        if (Program.ConfigurationInstance!.Base!.OscThings.ForceStartMediaStatus
            || (Processes.VrChatProcess is not null && !Program.ConfigurationInstance.Base.OscThings.ForceStartMediaStatus)) {
            OscMedia.StartMediaDetection();
        }

        try {
            foreach (var obj in Processes.SingleApplications) {
                if (obj.Value is null) continue;
                Logger.Information("Minimizing {Name}...", obj.Key);
                ShowWindow(obj.Value.MainWindowHandle, 6);
            }
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to minimize single processes");
        }

        try {
            foreach (var obj in Processes.MultiApplications) {
                if (obj.Value is null) continue;
                foreach (var process in obj.Value) {
                    Logger.Information("Minimizing {Name}...", process.ProcessName);
                    ShowWindow(process.MainWindowHandle, 6);
                }
            }
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to minimize multiple processes");
        }
        
        await Task.CompletedTask;
    }
}