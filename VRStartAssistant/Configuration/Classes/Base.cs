namespace VRStartAssistant.Configuration.Classes; 

public class Base {
    public int ConfigVersion { get; set; } = Vars.TargetConfigVersion;
    public Audio Audio { get; set; } = new();
    public WinXSO WinXSO { get; set; } = new();
}