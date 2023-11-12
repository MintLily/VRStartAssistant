// Came from https://github.com/RequiDev/AudioDeviceWatcher
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using Serilog;
using VRStartAssistant.COM;

namespace VRStartAssistant;

internal class AudioSwitch : IMMNotificationClient {
    // private string _presetInputDevice = null!;
    private readonly string _presetOutputDevice = "{0.0.0.00000000}.{7bcc81d8-2edd-4045-9234-2d8bbc8b4847}";

    private readonly MMDeviceEnumerator _deviceEnumerator;
    private readonly PolicyConfigClient _policyConfigClient;

    public AudioSwitch() {
        Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "AudioSwitcher", "Automatically switches to VR USB Audio");
        _deviceEnumerator = new MMDeviceEnumerator();
        _policyConfigClient = new PolicyConfigClient();
        _deviceEnumerator.RegisterEndpointNotificationCallback(this);
    }

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) {
        if (role != Role.Communications)
            return;

        switch (flow) {
            /*case DataFlow.Capture when !string.IsNullOrEmpty(_presetInputDevice) &&
                                       defaultDeviceId != _presetInputDevice &&
                                       _deviceEnumerator.GetDevice(_presetInputDevice) != null:
                SetDefaultAudioDevice(_presetInputDevice);
                break;*/
            case DataFlow.Render when !string.IsNullOrEmpty(_presetOutputDevice) &&
                                      defaultDeviceId != _presetOutputDevice &&
                                      _deviceEnumerator.GetDevice(_presetOutputDevice) != null:
                SetDefaultAudioDevice(_presetOutputDevice);
                Log.Information("Audio device changed to {Device}", _presetOutputDevice);
                break;
        }

        Console.Beep();
    }

    private void SetDefaultAudioDevice(string deviceId) {
        for (var i = 0; i <= (int)Role.Communications; i++) {
            _policyConfigClient.SetDefaultEndpoint(deviceId, (Role)i);
        }
    }

    #region not important

    public void OnDeviceStateChanged(string deviceId, DeviceState newState) {
        // not important
    }

    public void OnDeviceAdded(string pwstrDeviceId) {
        // not important
    }

    public void OnDeviceRemoved(string deviceId) {
        // not important
    }

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) {
        // not important
    }

    #endregion
}