namespace VRStartAssistant.Configuration.Classes;

public class OscThings {
    public int ListeningPort { get; init; } = 9000;
    public int SendingPort { get; init; } = 9001;
    public bool ShowMediaStatus { get; init; } = true;
    public List<string> CustomBlockWordsContains { get; init; } = [];
    public List<string> CustomBlockWordsEquals { get; init; } = [];
    public int SecondToAutoHideChatBox { get; init; } = 2;
}