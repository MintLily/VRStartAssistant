using RestSharp;
using Serilog;

namespace VRStartAssistant.Features.Integrations;

public class HASS {
    public HASS() => Logger.Information("Setting up module :: {Description}", "Controls Home Assistant devices");
    private static readonly ILogger Logger = Log.ForContext<HASS>();
    private static int _requestCount = 0;

    public static async Task ToggleBaseStations(bool toggleOff = false) {
        _requestCount = 0;
        var hass = Program.ConfigurationInstance.Base.HASS;
        var client = new RestClient(hass.Host + "api/");
        if (Vars.Verbose)
            Logger.Information("[{0}] RestClient URI: {1}", "VERBOSE", $"{hass.Host}api/");

        // Home Assistant Authorization
        try {
            Logger.Information("Connecting to Home Assistant...");
            if (Vars.Verbose)
                Logger.Information("[{0}] Assigned Headers", "VERBOSE");
            client.AddDefaultHeaders(new Dictionary<string, string> {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {hass.Token}" }
            });
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to connect to Home Assistant. Most likely caused by an invalid token.");
            return;
        }

        if (hass.ControlSwitches) {
            // Toggle Base Stations - or Switches
            try {
                var dic = new Dictionary<int, RestRequest>();
                for (var i = 0; i < hass.ToggleSwitchEntityIds.Count; i++) // create requests based on how many base stations there are
                    dic.Add(i, new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post));
                if (Vars.Verbose)
                    Logger.Information("[{0}] Added RestRequest to Dictionary: {1}", "VERBOSE", $"services/switch/{(toggleOff ? "turn_off" : "turn_on")}");

                Logger.Information("Sending requests - Turning Base Stations {0}", toggleOff ? "off" : "on");
                var requestCount = 0;
                foreach (var (key, value) in dic) {
                    if (hass.ToggleSwitchEntityIds[key] == "") // if entry is empty, skip
                        continue;

                    var jsonObject = $"{{\"entity_id\": \"switch.{hass.ToggleSwitchEntityIds[key]}\"}}";

                    value.AddJsonBody(jsonObject);
                    if (Vars.Verbose)
                        Logger.Information("[{0}] Added value ({1}) to RestRequest", "VERBOSE", jsonObject);

                    await client.PostAsync(value);
                    if (Vars.Verbose)
                        Logger.Information("[{0}] Sent Post Request", "VERBOSE");
                    requestCount++;
                }

                Logger.Information("{0} Requests sent.", requestCount);
            }
            catch (Exception e) {
                Logger.Error(e, "Failed to toggle Base Stations");
            }
        }

        if (hass.ControlLights) {
            // Set Light Brightness + Color
            try {
                var dic = new Dictionary<int, RestRequest>();
                for (var i = 0; i < hass.LightEntityIds.Count; i++)
                    dic.Add(i, new RestRequest($"services/light/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post));
                if (Vars.Verbose)
                    Logger.Information("[{0}] Added RestRequest to Dictionary: {1}", "VERBOSE", $"services/light/{(toggleOff ? "turn_off" : "turn_on")}");

                Logger.Information("Sending requests - Setting Light Brightness + Color");
                foreach (var (key, value) in dic) {
                    if (hass.LightEntityIds[key] == "") // if entry is empty, skip
                        continue;

                    var jsonObject = $"{{\"entity_id\": \"light.{hass.LightEntityIds[key]}\", \"brightness\": {hass.LightBrightness}, \"rgb_color\": [{hass.LightColor[0]}, {hass.LightColor[1]}, {hass.LightColor[2]}]}}";

                    value.AddJsonBody(jsonObject);
                    if (Vars.Verbose)
                        Logger.Information("[{0}] Added value ({1}) to RestRequest", "VERBOSE", jsonObject);

                    // await Task.Delay(TimeSpan.FromMilliseconds(500)); // delay between requests to prevent overloading the server
                    // Logger.Debug("Hitting request {0} with JSON:\n{1}", requestCount, jsonObject);
                    await client.PostAsync(value);
                    if (Vars.Verbose)
                        Logger.Information("[{0}] Sent Post Request", "VERBOSE");
                    _requestCount++;
                }
            }
            catch (Exception e) {
                Logger.Error(e, "Failed to set light brightness + color");
            }
        }

        Logger.Information("{0} Requests sent.", _requestCount);

        client.Dispose();
        if (Vars.Verbose)
            Logger.Information("[{0}] Disposed HASS Client", "VERBOSE");
    }
}