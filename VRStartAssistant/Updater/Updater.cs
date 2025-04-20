using System.Diagnostics;
using System.Text.Json;

namespace VRStartAssistant.Updater;

internal class Updater {
    public Updater UpdaterClass() => this;
    private const string GitHubApiUrl = "https://api.github.com/repos/MintLily/VRStartAssistant/releases/latest";
    private string? _apiResponse;
    public static bool IsUpdateAvailable { get; private set; }

    internal async Task Start() {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", $"MintLily/{Vars.AppName} v{Vars.AppVersion}");
        _apiResponse = await httpClient.GetStringAsync( GitHubApiUrl );
        var apiJson = JsonSerializer.Deserialize<Api>( _apiResponse );
        if (apiJson is null) {
            httpClient.Dispose();
            return;
        }

        var latestVersion = apiJson.tag_name;
        if (Vars.AppVersion != latestVersion) {
            IsUpdateAvailable = true;
            httpClient.Dispose();
            // TODO: Full updater EXE like WindowsXSO
            return;
        }
        
        httpClient.Dispose();
    }
    
    internal void OpenWebsite() => Process.Start("https://github.com/MintLily/VRStartAssistant/releases/latest");
}
