using Serilog;
using AudioSwitcher.AudioApi.CoreAudio;

namespace VRStartAssistant;

public class AudioSwitch {
    public AudioSwitch() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "AudioSwitcher", "Automatically switches to specific audio devices");
    
    private readonly string _indexUsbAudioAdapterGuid = "1f6321e0-59f8-4a24-933c-6c3918f6262a";
    private readonly string _soudcoreVrP10Guid = "feef26a6-db77-42e9-837d-4152d82fdac6";

    public async Task Start() {
        Log.Information("Attempting to set default audio device to {Device}...", _indexUsbAudioAdapterGuid);
        try {
            var controller = new CoreAudioController();
            // var devices = await controller.GetPlaybackDevicesAsync(DeviceState.Active);
            // foreach (var dev in devices) {
            //     Console.WriteLine($"{dev.Name} - {dev.Id} - Is Muted: {dev.IsMuted}");
            // }
            var device = await controller.GetDeviceAsync(Guid.Parse(_indexUsbAudioAdapterGuid));
            controller.DefaultPlaybackDevice = device;
            Log.Information("[{0}] Set audio device", "AUDIO");
        }
        catch (Exception e) {
            Log.Error(e, "Failed to set default audio device");
        }
    }
}
