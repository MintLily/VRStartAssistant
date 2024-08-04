using AudioSwitcher.AudioApi;
using Serilog;
using AudioSwitcher.AudioApi.CoreAudio;

namespace VRStartAssistant;

public class AudioSwitch {
    public AudioSwitch() => Logger.Information("Setting up module :: {Description}", "Automatically switches to specific audio devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(AudioSwitch));

    public static void Start(bool showDebug = false) {
        try {
            var controller = new CoreAudioController();
            if (showDebug) {
                Logger.Information("Looking for devices...");
                var devices = controller.GetPlaybackDevicesAsync(DeviceState.Active).GetAwaiter().GetResult();
                foreach (var dev in devices) {
                    Logger.Information("{0} - {1} - Is Muted: {2} - Volume: {3}", dev.FullName, dev.Id, dev.IsMuted, dev.Volume);
                }
                Logger.Information("Is Running Debug ... Not changing audio device");
                return;
            }
            Logger.Information("Attempting to set default audio device to {Device}...", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Name);
            var device = controller.GetDeviceAsync(Guid.Parse(Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Guid)).GetAwaiter().GetResult();
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
    
    public static void SwitchBack() {
        try {
            var controller = new CoreAudioController();
            Logger.Information("Attempting to set default audio device to {Device}...", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.SwitchBackAudioDevice].Name);
            var device = controller.GetDeviceAsync(Guid.Parse(Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.SwitchBackAudioDevice].Guid)).GetAwaiter().GetResult();
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.SwitchBackAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
}
