namespace VRStartAssistant.Configuration.Classes; 

public class Base {
    public int ConfigVersion { get; set; } = Vars.TargetConfigVersion;
    public Audio Audio { get; init; } = new();
    public bool SetBaseDirectoryToDev { get; init; } = false;
    public WinXSO WinXSO { get; init; } = new();
    public HASS HASS { get; init; } = new();
    public bool ShowMediaStatus { get; init; } = true;
    public bool RunSecretApp1 { get; init; } = false;
    public bool RunVrcVideoCacher { get; init; } = false;
    public bool RunAdGoBye { get; init; } = false;
    public bool RunHOSCY { get; init; } = false;
}