namespace VRStartAssistant.Configuration.Classes; 

public class Base {
    public int ConfigVersion { get; set; } = Vars.TargetConfigVersion;
    public Audio Audio { get; init; } = new();
    public WinXSO WinXSO { get; init; } = new();
    public HASS HASS { get; init; } = new();
    public OscThings OscThings { get; init; } = new();
    public bool RunSecretApp1 { get; init; } = false;
    public bool RunVrcVideoCacher { get; init; } = false;
    public bool RunAdGoBye { get; init; } = false;
    public bool RunHOSCY { get; init; } = false;
    public bool RunHeartRateOnStream { get; init; } = false;
}