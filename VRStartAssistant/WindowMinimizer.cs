using System.Runtime.InteropServices;
using Serilog;

namespace VRStartAssistant; 

public class WindowMinimizer {
    public WindowMinimizer() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "Window Minimizer", "Functions to minimize windows");
    
    // [DllImport("user32.dll", EntryPoint = "FindWindow")]
    // private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();
}