using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes; 

public class Audio {
    public int DefaultAudioDevice { get; init; } = 0;
    public List<AudioDevices> AudioDevices { get; init; } = [];
    public int SwitchBackAudioDevice { get; init; } = 0;
}

public class AudioDevices {
    public int Id { get; set; } = 0;
    public string Name { get; init; } = "Device";
    [JsonPropertyName("GUID")] public string Guid { get; init; } = "00000000-0000-0000-0000-000000000000";
}