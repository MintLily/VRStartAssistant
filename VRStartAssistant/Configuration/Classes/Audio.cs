using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes; 

public class Audio {
    public int DefaultAudioDevice { get; set; } = 0;
    public List<AudioDevices> AudioDevices { get; set; } = [];
}

public class AudioDevices {
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "Device";
    [JsonPropertyName("GUID")] public string Guid { get; set; } = "00000000-0000-0000-0000-000000000000";
}