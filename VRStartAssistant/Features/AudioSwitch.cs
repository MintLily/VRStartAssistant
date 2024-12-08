using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Serilog;
using VRStartAssistant.Configuration;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Features;

public class AudioSwitch {
    public AudioSwitch() => Logger.Information("Setting up module :: {Description}", "Automatically switches to specific audio devices");
    private static readonly ILogger Logger = Log.ForContext<AudioSwitch>();
    private static readonly CoreAudioController Controller = new();

    public static void Start(bool showDebug = false) {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        try {
            if (showDebug) {
                GetAudioDevices();
                Logger.Information("Is Running Debug ... Not changing audio device");
                return;
            }
            
            Logger.Information("Attempting to set default audio device to {Device}...", conf.AudioDevices[conf.DefaultAudioDevice].Name);
            var device = Controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.DefaultAudioDevice].Guid)).GetAwaiter().GetResult();
            Controller.DefaultPlaybackDevice = device;
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
            Logger.Information("Attempting to set default audio device to {Device}...", conf.AudioDevices[conf.SwitchBackAudioDevice].Name);
            var device = Controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.SwitchBackAudioDevice].Guid)).GetAwaiter().GetResult();
            Controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", conf.AudioDevices[conf.SwitchBackAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }

    public static void GetAudioDevices() {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        List<AudioDevices> audioDevices = [];
        Logger.Information("Looking for devices...");
        var devices = Controller.GetPlaybackDevicesAsync(DeviceState.Active).GetAwaiter().GetResult();
        foreach (var dev in devices) {
            Logger.Information("{0} - {1} - Is Muted: {2} - Volume: {3}", dev.FullName, dev.Id, dev.IsMuted, dev.Volume);
            
            if (!conf.ApplyAllDevicesToList)
                continue;
            
            conf.AudioDevices.Add(new AudioDevices {
                Id = conf.AudioDevices.Count,
                Name = dev.FullName,
                Guid = dev.Id.ToString()
            });
        }

        if (!conf.ApplyAllDevicesToList)
            return;
        
        Logger.Information("Saved devices to configuration file...");
        conf.ApplyAllDevicesToList = false;
        Program.ConfigurationInstance.Save();
    }
}
