#if DEBUG
using System.Text.Json;
using Serilog;

namespace VRStartAssistant.Configuration.debug;

public class debugconfig {
    public DebugBase? Base { get; private set; }
    private readonly ILogger _logger = Log.ForContext<debugconfig>();

    internal void Load() {
        var hasFile = File.Exists("debug.config.json");
        
        var defaultConfig = new DebugBase {
            ConfigVersion = 1,
            RunApps = false
        };
        
        bool update;
        DebugBase? config = null;
        if (hasFile) {
            var oldJson = File.ReadAllText("debug.config.json");
            config = JsonSerializer.Deserialize<DebugBase>(oldJson);
            if (config?.ConfigVersion == 1) {
                Base = config;
                update = false;
            }
            else {
                update = true;
                config!.ConfigVersion = 1;
            }
        }
        else {
            update = true;
        }

        var json = JsonSerializer.Serialize(config ?? defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("debug.config.json", json);
        //_logger.Information("{0} VRStartAssistant.config.json", update ? "Updated" : hasFile ? "Loaded" : "Created");
        Base = config ?? defaultConfig;
    }
    
    public void Save() => File.WriteAllText("debug.config.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}

public class DebugBase {
    public int ConfigVersion { get; set; } = 1;
    public bool RunApps { get; set; } = false;
}
#endif