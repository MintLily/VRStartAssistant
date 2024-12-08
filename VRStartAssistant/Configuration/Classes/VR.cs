using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes;

public class VR {
    [JsonPropertyName("Launch With SteamVR")] public bool AutoLaunchWithSteamVr { get; init; }
    [JsonPropertyName("hasRegistered (do not change)")] public bool HasRegistered { get; set; }
}