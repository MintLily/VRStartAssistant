using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class Processes {
    public Processes() => Logger.Information("Setting up module :: {Description}", "Process Module");
    private static readonly ILogger Logger = Log.ForContext(typeof(Processes));
    
    public static Process? SteamVrProcess;
    public static Process? VrChatProcess;
    public static Process? VrcxProcess;
    public static Process? VrcVideoCacher;
    public static Process? AdGoBye;
}