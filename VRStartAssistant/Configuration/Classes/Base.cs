using System.Text.Json.Serialization;

namespace VRStartAssistant.Configuration.Classes; 

public class Base {
    public int ConfigVersion { get; set; } = Vars.TargetConfigVersion;
    public int ConsoleOutputMaxLines { get; init; } = 9;
    public VR VR { get; init; } = new();
    public Audio Audio { get; init; } = new();
    public HASS HASS { get; init; } = new();
    public OscMusic OscMusic { get; init; } = new();
    public List<Programs> Programs { get; init; } = [];
}