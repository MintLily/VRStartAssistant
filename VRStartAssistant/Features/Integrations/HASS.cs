using RestSharp;
using Serilog;
using Serilog.Events;
using VRStartAssistant.Utils;

namespace VRStartAssistant.Features.Integrations;

public class HASS {
    private static int _requestCount = 0;

    public static async Task ToggleBaseStations(bool toggleOff = false) {
        _requestCount = 0;
        var hass = Program.ConfigurationInstance.Base.HASS;
        var client = new RestClient(hass.Host + "api/");

        // Home Assistant Authorization
        Try.Catch(() => {
            client.AddDefaultHeaders(new Dictionary<string, string> {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {hass.Token}" }
            });
        }, true);

        if (hass.ControlSwitches) {
            // Toggle Base Stations - or Switches
            await Try.Catch(async() => {
                var dic = new Dictionary<int, RestRequest>();
                for (var i = 0; i < hass.ToggleSwitchEntityIds.Count; i++) // create requests based on how many base stations there are
                    dic.Add(i, new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post));
                var requestCount = 0;
                foreach (var (key, value) in dic) {
                    if (hass.ToggleSwitchEntityIds[key] == "") // if entry is empty, skip
                        continue;

                    var jsonObject = $"{{\"entity_id\": \"switch.{hass.ToggleSwitchEntityIds[key]}\"}}";

                    value.AddJsonBody(jsonObject);
                    await client.PostAsync(value);
                    requestCount++;
                }
            }, true);
        }

        if (hass.ControlLights) {
            // Set Light Brightness + Color
            await Try.Catch(async () => {
                var dic = new Dictionary<int, RestRequest>();
                for (var i = 0; i < hass.LightEntityIds.Count; i++)
                    dic.Add(i, new RestRequest($"services/light/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post));
                
                foreach (var (key, value) in dic) {
                    if (hass.LightEntityIds[key] == "") // if entry is empty, skip
                        continue;

                    var jsonObject = $"{{\"entity_id\": \"light.{hass.LightEntityIds[key]}\", \"brightness\": {hass.LightBrightness}, \"rgb_color\": [{hass.LightColor[0]}, {hass.LightColor[1]}, {hass.LightColor[2]}]}}";

                    value.AddJsonBody(jsonObject);
                    await client.PostAsync(value);
                    _requestCount++;
                }
            }, true);
        }

        client.Dispose();
    }
}