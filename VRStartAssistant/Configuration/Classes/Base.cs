namespace VRStartAssistant.Configuration.Classes; 

public class Base {
    public int ConfigVersion { get; set; } = Vars.TargetConfigVersion;
    public Audio Audio { get; set; } = new();
    public WinXSO WinXSO { get; set; } = new();
    public bool RunSecretApp1 { get; set; } = false;
    public bool RunVrcVideoCacher { get; set; } = false;
    public bool RunAdGoBye { get; set; } = false;
    public bool RunBetterIndexFinger { get; set; } = false;
}