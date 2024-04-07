namespace VRStartAssistant.Configuration.Classes;

public class HASS {
    public string Host { get; set; } = "";
    public string Token { get; set; } = "";
    public List<string> BaseStationEntityIds { get; set; } = new();
    public bool ControlLights { get; set; } = true;
    public List<string> HueLightEntityIds { get; set; } = new();
    public List<string> ExtraHueLightEntityIds { get; set; } = new();
    public float LightBrightness { get; set; } = 0.0f;
    public float[] LightColor { get; set; } = [255f, 255f, 255f];
}