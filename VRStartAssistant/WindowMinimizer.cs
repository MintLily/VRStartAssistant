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
        await Task.Delay(TimeSpan.FromSeconds(Vars.IsDebug ? 5 : 30));
        
        ShowWindow(GetConsoleWindow(), 0); // Hide this console window

        if (Processes.VrChatProcess is not null) {
            Logger.Information("Minimizing VRChat...");
            ShowWindow(Processes.VrChatProcess.MainWindowHandle, 6);
            OscMedia.StartMediaDetection();
            // OscMedia.StartOscMediaThread();
        }
    }
}