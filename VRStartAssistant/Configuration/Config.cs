using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Configuration; 

public static class Config {
    public static Base Base { get; set; } = Load();
    private static readonly ILogger Logger = Log.ForContext(typeof(Config));

    private static void Start() {
        var update = false;
        if (File.Exists("VRStartAssistant.config.json")) {
            if (Base.ConfigVersion == Vars.TargetConfigVersion) return;
            // else continue to update config
            update = true;
        }
        
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
        
        var config = new Base {
            ConfigVersion = 2,
            Audio = audio,
            WinXSO = wxso
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("VRStartAssistant.config.json", json);
        Logger.Information($"Config file {(update ? "updated" : "created")} in " + "{0}.", Vars.BaseDir);
    }
    
    private static Base Load() {
        Start();
        return JsonSerializer.Deserialize<Base>(File.ReadAllText("VRStartAssistant.config.json")) ?? throw new Exception();
    }
    
    public static void Save() => File.WriteAllText("VRStartAssistant.config.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}