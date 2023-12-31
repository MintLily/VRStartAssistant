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
    
    public async Task DelayedMinimize() {
        await Task.Delay(TimeSpan.FromSeconds(30));
        
        ShowWindow(GetConsoleWindow(), 0); // Hide this console window

        if (Processes.VrChatProcess is not null) {
            Logger.Information("Minimizing VRChat...");
            ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);
        }
        
        if (Processes.WindowsTerminal is not null) {
            Logger.Information("Minimizing Windows Terminal...");
            ShowWindow(Processes.WindowsTerminal.MainWindowHandle, 6);
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
        
        if (Program.ConfigurationInstance.Base.RunBetterIndexFinger && Processes.BetterIndexFinger is not null) {
            Logger.Information("Minimizing BetterIndexFinger...");
            ShowWindow(Processes.BetterIndexFinger.MainWindowHandle, 6);
        }
        
        if (Processes.Oyasumi is not null) {
            Logger.Information("Minimizing Oyasumi...");
            ShowWindow(Processes.Oyasumi.MainWindowHandle, 6);
        }
    }
}