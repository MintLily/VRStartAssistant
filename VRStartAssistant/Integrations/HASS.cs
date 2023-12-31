using RestSharp;
using Serilog;

namespace VRStartAssistant.Integrations;

public class HASS {
    public HASS() => Logger.Information("Setting up module :: {Description}", "Controls Home Assistant devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(HASS));

    public static async Task ToggleBaseStations(bool toggleOff = false) {
        Logger.Information("Connecting to Home Assistant...");
        var client = new RestClient(Program.ConfigurationInstance.Base.HASS.Host + "api/");
        client.AddDefaultHeaders(new Dictionary<string, string> {
            { "Content-Type", "application/json" },
            { "Authorization", $"Bearer {Program.ConfigurationInstance.Base.HASS.Token}" }
        });
        
        var request1 = new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post);
        var request2 = new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post);
        var request3 = new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post);
        
        request1.AddJsonBody($"{{\"entity_id\": \"switch.{Program.ConfigurationInstance.Base.HASS.BaseStationEntityId_1}\"}}");
        request2.AddJsonBody($"{{\"entity_id\": \"switch.{Program.ConfigurationInstance.Base.HASS.BaseStationEntityId_2}\"}}");
        request3.AddJsonBody($"{{\"entity_id\": \"switch.{Program.ConfigurationInstance.Base.HASS.BaseStationEntityId_3}\"}}");
        
        Logger.Information("Sending requests - Turning Base Stations {0}", toggleOff ? "off" : "on");
        await client.PostAsync(request1);
        await client.PostAsync(request2);
        await client.PostAsync(request3);
        Logger.Information("Requests sent.");
        client.Dispose();
    }
}