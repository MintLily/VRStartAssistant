using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes;

public class Programs {
    [JsonPropertyName("Name")] public string? Name { get; init; }
    [JsonPropertyName("EXE Path")] public string? ExePath { get; init; }
    [JsonPropertyName("Arguments")] public string? Arguments { get; init; }
    [JsonPropertyName("Start With VRSA")] public bool StartWithVrsa { get; init; }
    [JsonPropertyName("Start Minimized")] public bool StartMinimized { get; init; }
    [JsonPropertyName("Has Multiple Processes")] public bool HasMultiProcesses { get; init; }
    [JsonPropertyName("Process Name")] public string? ProcessName { get; init; }
    [JsonPropertyName("Fallback Process Starting Needed")] public bool FallbackProcessStartingNeeded { get; init; }
}