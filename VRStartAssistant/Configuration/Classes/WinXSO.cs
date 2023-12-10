using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes;

public class WinXSO {
    [JsonPropertyName("WindowsXSO Settings")] public Settings Settings { get; set; } = new();
}

public class Settings {
    [JsonPropertyName("Applications")] public List<string> Applications { get; set; } = ["discord", "vesktop"];
    public bool Whitelist { get; set; } = true;
}