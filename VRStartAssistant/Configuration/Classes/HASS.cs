namespace VRStartAssistant.Configuration.Classes;

public class HASS {
    public string Host { get; set; } = "";
    public string Token { get; set; } = "";
    public bool ControlSwitches { get; set; } = true;
    public List<string> ToggleSwitchEntityIds { get; init; } = [];
    public bool ControlLights { get; set; } = true;
    public List<string> LightEntityIds { get; init; } = [];
    public float LightBrightness { get; init; } = 0.0f;
    public float[] LightColor { get; init; } = [255f, 255f, 255f];
}