using System.Text.Json;
using Serilog;
using VRStartAssistant.Configuration.Classes;

namespace VRStartAssistant.Configuration; 

public static class Config {
    public static Base Base { get; set; } = Load();

    private static void Start() {
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "config.json"))) return;
        
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
            }
        };
        
        var audio = new Audio {
            DefaultAudioDevice = 0,
            AudioDevices = audioDevices
        };
        
        var config = new Base {
            ConfigVersion = 1,
            Audio = audio
        };
        
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "config.json"), json);
        Log.Information("[{0}] Config file created.", "CONFIG");
    }
    
    private static Base Load() {
        Start();
        return JsonSerializer.Deserialize<Base>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"))) ?? throw new Exception();
    }
    
    public static void Save() => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "config.json"), JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}