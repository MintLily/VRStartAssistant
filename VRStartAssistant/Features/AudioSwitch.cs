using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Serilog;
using Serilog.Events;
using VRStartAssistant.Configuration;
using VRStartAssistant.Configuration.Classes;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Features;

public class AudioSwitch {
    private static readonly CoreAudioController Controller = new();

    public static void Start(bool showDebug = false) {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        Try.Catch(() => {
            var device = Controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.DefaultAudioDevice].Guid)).GetAwaiter().GetResult();
            Controller.DefaultPlaybackDevice = device;
        }, true);
    }
    
    public static void SwitchBack() {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        
        if (conf.DefaultAudioDevice == conf.SwitchBackAudioDevice)
            return;

        Try.Catch(() => {
            var device = Controller.GetDeviceAsync(Guid.Parse(conf.AudioDevices[conf.SwitchBackAudioDevice].Guid)).GetAwaiter().GetResult();
            Controller.DefaultPlaybackDevice = device;
        }, true);
    }

    public static void GetAudioDevices(bool onlyPrint = false) {
        var conf = Program.ConfigurationInstance!.Base!.Audio;
        var tempLogger = Log.ForContext<AudioSwitch>();
        if (!onlyPrint && !conf.ApplyAllDevicesToList)
            return;
        
        var devices = Controller.GetPlaybackDevicesAsync(DeviceState.Active).GetAwaiter().GetResult();
        var audioDevices = new List<AudioDevices>();
        
        foreach (var audioDevice in devices) {
            tempLogger.Information("{0} - {1} - Is Muted: {2} - Volume: {3}", audioDevice.FullName, audioDevice.Id, audioDevice.IsMuted, audioDevice.Volume);
            audioDevices.Add(new AudioDevices {
                Id = audioDevices.Count,
                Name = audioDevice.FullName,
                Guid = audioDevice.Id.ToString()
            });
        }

        if (!onlyPrint) {
            conf.AudioDevices = audioDevices;
            conf.ApplyAllDevicesToList = false;
            Program.ConfigurationInstance.Save();
        }
        tempLogger.Information("Press any key to to resume application startup...");
        Console.ReadKey();
    }
}
