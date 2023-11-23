using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes; 

public class Audio {
    public int DefaultAudioDevice { get; set; }
    public List<AudioDevices> AudioDevices { get; set; }
}

public class AudioDevices {
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("GUID")] public string Guid { get; set; }
}