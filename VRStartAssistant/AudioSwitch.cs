﻿using AudioSwitcher.AudioApi;
using Serilog;
using AudioSwitcher.AudioApi.CoreAudio;

namespace VRStartAssistant;

public class AudioSwitch {
    public AudioSwitch() => Logger.Information("Setting up module :: {Description}", "Automatically switches to specific audio devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(AudioSwitch));

    public static async Task Start() {
        Logger.Information("Attempting to set default audio device to {Device}...", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Name);
        try {
            var controller = new CoreAudioController();
            if (Vars.IsDebug) {
                var devices = await controller.GetPlaybackDevicesAsync(DeviceState.Active);
                foreach (var dev in devices) {
                    Log.Debug("{0} - {1} - Is Muted: {2} - Volume: {3}", dev.FullName, dev.Id, dev.IsMuted, dev.Volume);
                }
                Logger.Information("Is Running Debug ... Not changing audio device");
                return;
            }
            var device = await controller.GetDeviceAsync(Guid.Parse(Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Guid));
            controller.DefaultPlaybackDevice = device;
            Logger.Information("Set audio device to {0}", Program.ConfigurationInstance.Base.Audio.AudioDevices[Program.ConfigurationInstance.Base.Audio.DefaultAudioDevice].Name);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set default audio device");
        }
    }
}
