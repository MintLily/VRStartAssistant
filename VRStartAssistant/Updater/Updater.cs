using Serilog;
using System.Text.Json;

namespace VRStartAssistant.Updater {
    internal class Updater {
        public Updater UpdaterClass() => this;
        private static readonly ILogger Logger = Log.ForContext(typeof(Updater));
        private const string GitHubApiUrl = "https://api.github.com/repos/MintLily/VRStartAssistant/releases/latest";
        private string? _apiResponse;

        internal async Task Start() {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"MintLily/{Vars.AppName} v{Vars.AppVersion}");
            _apiResponse = await httpClient.GetStringAsync( GitHubApiUrl );
            var apiJson = JsonSerializer.Deserialize<Api>( _apiResponse );
            if (apiJson is null) {
                Logger.Error("Failed to deserialize GitHub API response.");
                httpClient.Dispose();
                return;
            }

            var latestVersion = apiJson.tag_name;
            if (Vars.AppVersion != latestVersion) {
                Logger.Information("!!! {0} - Go to {1} to update !!!", "UPDATE AVAILABLE", "https://github.com/MintLily/VRStartAssistant/releases");
                httpClient.Dispose();
                // TODO: Full updater EXE like WindowsXSO
                return;
            }

            Logger.Information("You are running the latest version.");
            httpClient.Dispose();
        }
    }
}
