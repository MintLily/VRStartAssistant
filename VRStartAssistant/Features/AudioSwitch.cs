using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Serilog;

namespace VRStartAssistant.Features;

public class AudioSwitch {
    public AudioSwitch() => Logger.Information("Setting up module :: {Description}", "Automatically switches to specific audio devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(AudioSwitch));

    public static void Start(bool showDebug = false) {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
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
            Logger.Information("Attempting to set default audio device to {Device}...", conf.AudioDevices[conf.DefaultAudioDevice].Name);
            var device = controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.DefaultAudioDevice].Guid)).GetAwaiter().GetResult();
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", conf.AudioDevices[conf.DefaultAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
    
    public static void SwitchBack() {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        
        if (conf.DefaultAudioDevice == conf.SwitchBackAudioDevice)
            return;
        
        try {
            var controller = new CoreAudioController();
            Logger.Information("Attempting to set default audio device to {Device}...", conf.AudioDevices[conf.SwitchBackAudioDevice].Name);
            var device = controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.SwitchBackAudioDevice].Guid)).GetAwaiter().GetResult();
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", conf.AudioDevices[conf.SwitchBackAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
}
