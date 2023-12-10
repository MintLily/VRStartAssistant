#if DEBUG
using AudioSwitcher.AudioApi;
#endif
using Serilog;
using AudioSwitcher.AudioApi.CoreAudio;
using VRStartAssistant.Configuration;

namespace VRStartAssistant;

public class AudioSwitch {
    public AudioSwitch() => Logger.Information("Setting up module :: {Description}", "Automatically switches to specific audio devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(AudioSwitch));

    public async Task Start() {
        Logger.Information("Attempting to set default audio device to {Device}...", Config.Base.Audio.AudioDevices[Config.Base.Audio.DefaultAudioDevice].Name);
        try {
            var controller = new CoreAudioController();
#if DEBUG
            var devices = await controller.GetPlaybackDevicesAsync(DeviceState.Active);
            foreach (var dev in devices) {
                Log.Debug("{0} - {1} - Is Muted: {2} - Volume: {3}", dev.FullName, dev.Id, dev.IsMuted, dev.Volume);
            }
            Log.Information("[{0}] Is Debug Build ... Not changing audio device", "AUDIO");
            return;
#endif
            var device = await controller.GetDeviceAsync(Guid.Parse(Config.Base.Audio.AudioDevices[Config.Base.Audio.DefaultAudioDevice].Guid));
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", Config.Base.Audio.AudioDevices[Config.Base.Audio.DefaultAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
}
