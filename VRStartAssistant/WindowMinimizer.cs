using System.Runtime.InteropServices;
using Serilog;

namespace VRStartAssistant; 

public class WindowMinimizer {
    public WindowMinimizer() => Logger.Information("Setting up module :: {Description}", "Functions to minimize windows");
    
    private static readonly ILogger Logger = Log.ForContext(typeof(WindowMinimizer));
    
    // [DllImport("user32.dll", EntryPoint = "FindWindow")]
    // private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();
    
    public static async Task DelayedMinimize() {
        await Task.Delay(TimeSpan.FromSeconds(30));
        
        ShowWindow(GetConsoleWindow(), 0); // Hide this console window

        if (Processes.VrChatProcess is not null) {
            Logger.Information("Minimizing VRChat...");
            ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);
            OscMedia.StartMediaDetection();
            // OscMedia.StartOscMediaThread();
        }
        
        // if (Processes.WindowsTerminal is not null) {
        //     Logger.Information("Minimizing Windows Terminal...");
        //     foreach (var terms in Processes.WindowsTerminal) {
        //         ShowWindow(terms.MainWindowHandle, 6);
        //     }
        // }
        
        if (Processes.HOSCY is not null) {
            Logger.Information("Minimizing HOSCY...");
            ShowWindow(Processes.HOSCY.MainWindowHandle, 6);
        }
        
        // if (Program.ConfigurationInstance.Base.RunVrcVideoCacher && Processes.VrcVideoCacher is not null) {
        //     Logger.Information("Minimizing VRCVideoCacher...");
        //     ShowWindow(Processes.VrcVideoCacher.MainWindowHandle, 6);
        // }
        //
        // if (Program.ConfigurationInstance.Base.RunAdGoBye && Processes.AdGoBye is not null) {
        //     Logger.Information("Minimizing AdGoBye...");
        //     ShowWindow(Processes.AdGoBye.MainWindowHandle, 6);
        // }
    }
}