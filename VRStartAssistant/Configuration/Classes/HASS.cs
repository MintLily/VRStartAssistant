namespace VRStartAssistant.Configuration.Classes;

public class HASS {
    public string Host { get; init; } = "";
    public string Token { get; init; } = "";
    public List<string> BaseStationEntityIds { get; init; } = new();
    public bool ControlLights { get; init; } = true;
    public List<string> HueLightEntityIds { get; init; } = new();
    public List<string> ExtraHueLightEntityIds { get; init; } = new();
    public float LightBrightness { get; init; } = 0.0f;
    public float[] LightColor { get; init; } = [255f, 255f, 255f];
}