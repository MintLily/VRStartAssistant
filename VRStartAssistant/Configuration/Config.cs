using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Configuration; 

public class Config {
    public Base? Base { get; private set; }
    private readonly ILogger _logger = Log.ForContext(typeof(Config));

    internal void Load() {
        var hasFile = File.Exists("VRStartAssistant.config.json");
        
        var defaultConfig = new Base {
            ConfigVersion = Vars.TargetConfigVersion,
            Audio = new Audio {
                DefaultAudioDevice = 0,
                AudioDevices = [],
                SwitchBackAudioDevice = 1
            },
            WinXSO = new WinXSO {
                Settings = new Settings {
                    Applications = [
                        "discord",
                        "discordptb",
                        "discordcanary",
                        "vesktop"
                    ],
                    Whitelist = true
                }
            },
            HASS = new HASS {
                Host = "http://127.0.0.1:8123/",
                Token = "",
                BaseStationEntityIds = [""],
                ControlLights = false,
                HueLightEntityIds = [""],
                ExtraHueLightEntityIds = [""],
                LightBrightness = 0.0f,
                LightColor = [0, 0, 0]
            },
            OscThings = new OscThings {
                ShowMediaStatus = false,
                CustomBlockWordsContains = [],
                CustomBlockWordsEquals = [],
                SecondToAutoHideChatBox = 2
            },
            RunSecretApp1 = true,
            RunVrcVideoCacher = false,
            RunAdGoBye = false,
            RunHOSCY = false,
            RunHeartRateOnStream = false,
        };
        
        bool update;
        Base? config = null;
        if (hasFile) {
            var oldJson = File.ReadAllText("VRStartAssistant.config.json");
            config = JsonSerializer.Deserialize<Base>(oldJson);
            if (config?.ConfigVersion == Vars.TargetConfigVersion) {
                Base = config;
                update = false;
            } else {
                update = true;
                config!.ConfigVersion = Vars.TargetConfigVersion;
            }
        } else {
            update = true;
        }
        
        var json = JsonSerializer.Serialize(config ?? defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("VRStartAssistant.config.json", json);
        _logger.Information("{0} VRStartAssistant.config.json", update ? "Updated" : hasFile ? "Loaded" : "Created");
        Base = config ?? defaultConfig;
    }
    
    // public void Save() => File.WriteAllText("VRStartAssistant.config.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}