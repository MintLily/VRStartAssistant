namespace VRStartAssistant.Configuration.Classes;

public class OscThings {
    public bool ShowMediaStatus { get; init; } = true;
    public List<string> CustomBlockWordsContains { get; init; } = [];
    public List<string> CustomBlockWordsEquals { get; init; } = [];
    public int SecondToAutoHideChatBox { get; init; } = 2;
}