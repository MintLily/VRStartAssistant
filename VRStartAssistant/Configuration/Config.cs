using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Configuration;

public class Config {
    public Base? Base { get; private set; }
    private readonly ILogger _logger = Log.ForContext<Config>();

    internal void Load() {
        var hasFile = File.Exists("VRStartAssistant.config.json");

        var defaultConfig = new Base {
            ConfigVersion = Vars.TargetConfigVersion,
            VR = new VR {
                AutoLaunchWithSteamVr = false,
                HasRegistered = false
            },
            Audio = new Audio {
                DefaultAudioDevice = 0,
                ApplyAllDevicesToList = false,
                AudioDevices = [],
                SwitchBackAudioDevice = 1
            },
            HASS = new HASS {
                Host = "http://127.0.0.1:8123/",
                Token = "",
                ToggleSwitchEntityIds = [""],
                ControlLights = false,
                LightEntityIds = [""],
                LightBrightness = 0.0f,
                LightColor = [0, 0, 0]
            },
            OscMusic = new OscMusic {
                ListeningPort = 9001,
                SendingPort = 9000,
                ShowMediaStatus = false,
                ForceStartMediaStatus = false,
                CustomBlockWordsContains = [],
                CustomBlockWordsEquals = [ "Up next", "DJ X" ],
                SecondsToAutoHideChatBox = 2
            },
            Programs = [
                new Programs {
                    Name = "VRCX",
                    ExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VRCX", "VRCX.exe"),
                    Arguments = "",
                    StartWithVrsa = true,
                    StartMinimized = true,
                    HasMultiProcesses = true,
                    ProcessName = "vrcx",
                    FallbackProcessStartingNeeded = true
                }
            ]
        };

        bool update;
        Base? config = null;
        if (hasFile) {
            var oldJson = File.ReadAllText("VRStartAssistant.config.json");
            config = JsonSerializer.Deserialize<Base>(oldJson);
            if (config?.ConfigVersion == Vars.TargetConfigVersion) {
                Base = config;
                update = false;
            }
            else {
                update = true;
                config!.ConfigVersion = Vars.TargetConfigVersion;
            }
        }
        else {
            update = true;
        }

        var json = JsonSerializer.Serialize(config ?? defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("VRStartAssistant.config.json", json);
        _logger.Information("{0} VRStartAssistant.config.json", update ? "Updated" : hasFile ? "Loaded" : "Created");
        Base = config ?? defaultConfig;
    }

    public void Save() => File.WriteAllText("VRStartAssistant.config.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));

    public string ToJson() => JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true });
}