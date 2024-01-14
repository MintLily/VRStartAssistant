﻿using RestSharp;
using Serilog;
using VRStartAssistant.Configuration;

namespace VRStartAssistant.Integrations;

public class HASS {
    public HASS() => Logger.Information("Setting up module :: {Description}", "Controls Home Assistant devices");
    private static readonly ILogger Logger = Log.ForContext(typeof(HASS));

    public static async Task ToggleBaseStations(bool toggleOff = false) {
        var hass = Program.ConfigurationInstance.Base.HASS;
        var client = new RestClient(hass.Host + "api/");

        // Home Assistant Authorization
        try {
            Logger.Information("Connecting to Home Assistant...");
            client.AddDefaultHeaders(new Dictionary<string, string> {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {hass.Token}" }
            });
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to connect to Home Assistant");
        }

        // Toggle Base Stations
        try {
            var dic = new Dictionary<int, RestRequest>();
            for (var i = 0; i < hass.BaseStationEntityIds.Count; i++) // create requests based on how many base stations there are
                dic.Add(i, new RestRequest($"services/switch/{(toggleOff ? "turn_off" : "turn_on")}", Method.Post));

            Logger.Information("Sending requests - Turning Base Stations {0}", toggleOff ? "off" : "on");
            var requestCount = 0;
            foreach (var (key, value) in dic) {
                if (hass.BaseStationEntityIds[key] == "") // if entry is empty, skip
                    continue;

                value.AddJsonBody($"{{\"entity_id\": \"switch.{hass.BaseStationEntityIds[key]}\"}}");
                // await Task.Delay(TimeSpan.FromMilliseconds(500));
                await client.PostAsync(value);
                requestCount++;
            }

            Logger.Information("{0} Requests sent.", requestCount);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to toggle Base Stations");
        }

        if (!hass.ControlLights) {
            client.Dispose();
            return;
        }

        // Set Light Brightness + Color
        try {
            var dic = new Dictionary<int, RestRequest>();
            for (var i = 0; i < hass.HueLightEntityIds.Count; i++) // create requests based on how many lights there are whilst including the accent light cutoff (not main lights)
                dic.Add(i, new RestRequest($"services/light/{(i > hass.AccentLightCutoff ? "turn_off" : "turn_on")}", Method.Post));

            Logger.Information("Sending requests - Setting Light Brightness + Color");
            var requestCount = 0;
            foreach (var (key, value) in dic) {
                if (hass.HueLightEntityIds[key] == "") // if entry is empty, skip
                    continue;
                requestCount++;

                var jsonObject = $"{{\"entity_id\": \"light.{hass.HueLightEntityIds[key]}\"{
                    (key > hass.AccentLightCutoff
                        ? ", \"brightness\": 0" // brute-force turn off accent lights
                        : $", \"brightness\": {hass.LightBrightness}, \"rgb_color\": [{hass.LightColor[0]}, {hass.LightColor[1]}, {hass.LightColor[2]}]")
                }}}";

                value.AddJsonBody(jsonObject);

                // await Task.Delay(TimeSpan.FromMilliseconds(500)); // delay between requests to prevent overloading the server
                // Logger.Debug("Hitting request {0} with JSON:\n{1}", requestCount, jsonObject);
                await client.PostAsync(value);
            }

            Logger.Information("{0} Requests sent.", requestCount);
        }
        catch (Exception e) {
            Logger.Error(e, "Failed to set light brightness + color");
        }

        client.Dispose();
    }
}