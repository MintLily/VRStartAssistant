using VRStartAssistant.Apps;

namespace VRStartAssistant;

public class ValidateAppPaths {
    public static void Go() {
        AdGoBye.ValidatePath();
        HeartrateMonitor.ValidatePath();
        HOSCY.ValidatePath();
        OSCLeash.ValidatePath();
        VRCVideoCacher.ValidatePath();
    }
}