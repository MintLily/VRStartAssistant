using Serilog;
using AudioSwitcher.AudioApi.CoreAudio;
using VRStartAssistant.Configuration;

namespace VRStartAssistant;

public class AudioSwitch {
    public AudioSwitch() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "AudioSwitcher", "Automatically switches to specific audio devices");

    public async Task Start() {
        Log.Information("[{0}] Attempting to set default audio device to {Device}...", "AUDIO", Config.Base.Audio.AudioDevices[Config.Base.Audio.DefaultAudioDevice].Guid);
        try {
            var controller = new CoreAudioController();
            // var devices = await controller.GetPlaybackDevicesAsync(DeviceState.Active);
            // foreach (var dev in devices) {
            //     Console.WriteLine($"{dev.Name} - {dev.Id} - Is Muted: {dev.IsMuted}");
            // }
            var device = await controller.GetDeviceAsync(Guid.Parse(Config.Base.Audio.AudioDevices[Config.Base.Audio.DefaultAudioDevice].Guid));
            controller.DefaultPlaybackDevice = device;
            Log.Information("[{0}] Set audio device", "AUDIO");
        }
        catch (Exception e) {
            Log.Error(e, "Failed to set default audio device");
        }
    }
}
