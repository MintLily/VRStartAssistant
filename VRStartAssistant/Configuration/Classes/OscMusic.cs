namespace VRStartAssistant.Configuration.Classes;

public class OscMusic {
    public int ListeningPort { get; set; } = 9000;
    public int SendingPort { get; set; } = 9001;
    public bool ShowMediaStatus { get; set; } = false;
    public bool ForceStartMediaStatus { get; init; } = false;
    public List<string> CustomBlockWordsContains { get; init; } = [];
    public List<string> CustomBlockWordsEquals { get; init; } = [];
    public int SecondsToAutoHideChatBox { get; set; } = 2;
}