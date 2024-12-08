using System.Diagnostics;
using Serilog;

namespace VRStartAssistant; 

public class Processes {
    public Processes() => Logger.Information("Setting up module :: {Description}", "Process Module");
    private static readonly ILogger Logger = Log.ForContext<Processes>();
    
    /* In-App Processes */
    public static Dictionary<string, Process?> SingleApplications = new();
    public static Dictionary<string, List<Process>?> MultiApplications = new();
    public static Process? SteamVrProcess;
    public static Process? VrChatProcess;
}