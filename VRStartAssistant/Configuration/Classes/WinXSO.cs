using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes;

public class WinXSO {
    [JsonPropertyName("WindowsXSO Settings")] public Settings Settings { get; init; } = new();
}

public class Settings {
    [JsonPropertyName("Applications")] public List<string> Applications { get; init; } = ["discord", "vesktop"];
    public bool Whitelist { get; init; } = true;
}