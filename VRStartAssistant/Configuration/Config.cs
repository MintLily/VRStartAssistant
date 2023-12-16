using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Configuration; 

public class Config {
    public Base Base { get; private set; }
    private readonly ILogger _logger = Log.ForContext(typeof(Config));

    public void Load() {
        var hasFile = File.Exists("VRStartAssistant.config.json");
        
        var wxso = new WinXSO {
            Settings = new() {
                Applications = new() {
                    "discord",
                    "discordptb",
                    "discordcanary",
                    "vesktop"
                },
                Whitelist = true
            }
        };
        
        var audioDevices = new List<AudioDevices> {
            new() {
                Id = 0,
                Name = "Index USB (Audio Adapter)",
                Guid = "1f6321e0-59f8-4a24-933c-6c3918f6262a"
            },
            new() {
                Id = 1,
                Name = "VR P10",
                Guid = "feef26a6-db77-42e9-837d-4152d82fdac6"
            },
            new() {
                Id = 2,
                Name = "JBL Charge 4",
                Guid = "f103d47f-c0c7-4e07-87a0-309c10c9abb0"
            }
        };
        
        var audio = new Audio {
            DefaultAudioDevice = 0,
            AudioDevices = audioDevices
        };
        
        var defaultConfig = new Base {
            ConfigVersion = 2,
            Audio = audio,
            WinXSO = wxso
        };
        
        bool update;
        Base config = null;
        if (hasFile) {
            var oldJson = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Config.json"));
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
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "VRStartAssistant.config.json"), json);
        _logger.Information("{0} VRStartAssistant.config.json", update ? "Updated" : hasFile ? "Loaded" : "Created");
        Base = config ?? defaultConfig;
    }
    
    public void Save() => File.WriteAllText("VRStartAssistant.config.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}