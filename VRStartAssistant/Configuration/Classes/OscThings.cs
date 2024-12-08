namespace VRStartAssistant.Configuration.Classes;

public class OscThings {
    public int ListeningPort { get; init; } = 9000;
    public int SendingPort { get; init; } = 9001;
    public bool ShowMediaStatus { get; init; } = false;
    public bool ForceStartMediaStatus { get; init; } = false;
    public List<string> CustomBlockWordsContains { get; init; } = [];
    public List<string> CustomBlockWordsEquals { get; init; } = [];
    public int SecondsToAutoHideChatBox { get; init; } = 2;
}