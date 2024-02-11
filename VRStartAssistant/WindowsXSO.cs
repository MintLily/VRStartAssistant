using System.Diagnostics;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using Serilog;
using VRStartAssistant.Apps;
using XSNotifications;
using XSNotifications.Enum;

namespace VRStartAssistant; 

public class WindowsXSO {
    public WindowsXSO() => Logger.Information("Setting up module :: {Description}", "Windows to XSOverlay Notification Relay");
    private static readonly ILogger Logger = Log.ForContext(typeof(WindowsXSO));

    private static UserNotificationListener? _listener;
    private static readonly List<uint> KnownNotifications = [];
    private static readonly List<string> TargetApplicationNames = Program.ConfigurationInstance.Base.WinXSO.Settings.Applications.ToList();
    private static readonly XSNotifier XsNotifier = new();
    
    public static async Task StartAsync() {
        _listener = UserNotificationListener.Current;
        var accessStatus = _listener.RequestAccessAsync().GetResults();

        var isInRestartMessage = false;
        switch (accessStatus) {
            case UserNotificationListenerAccessStatus.Allowed:
                Logger.Information("Notifications {0}.", "access granted");
                break;
            case UserNotificationListenerAccessStatus.Denied:
                Logger.Error("Notifications {0}.", "access denied");
                isInRestartMessage = true;
                Logger.Warning("Please grant access to notifications.");
                Console.WriteLine("----------------------------------------");
                Logger.Warning("<[{0}]>", "Windows 11");
                Logger.Warning("(System) Settings > Privacy & Security > Notifications (Section) > Allow apps to access notifications > ON (true)");
                Logger.Warning("<[{0}]>", "Windows 10");
                Logger.Warning("(System) Settings > Notifications & actions > Get notifications from apps and other senders > ON (true)");
                Logger.Warning("<[{0}]>", "BOTH");
                Logger.Warning("Make sure Focus Assist is OFF (false)");
                Logger.Warning("Once complete, restart this program.");
                Logger.Warning($"Press any key to exit {Vars.AppName}.");
                Console.ReadKey();
                break;
            case UserNotificationListenerAccessStatus.Unspecified:
                Logger.Warning("Notifications {0}.", "access unspecified");
                Logger.Warning("Notifications may not work as intended.");
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        if (isInRestartMessage)
            Process.GetCurrentProcess().Kill();
        
        var config = Program.ConfigurationInstance.Base!.WinXSO.Settings;
        // Logger.Information("Whitelist target applications: {0}", string.Join(", ", TargetApplicationNames!));
        
        Logger.Information($"Starting notification listener in {(config.Whitelist ? "Whitelist" : "Blacklist")} mode...");
        Logger.Information($"{(config.Whitelist ? "Allowing" : "Blocking")} target applications: " + "{0}", string.Join(", ", config.Applications!));
        Program.ChangeConsoleTitle();
        while (true) { // Keep the program running
            
            // Check if SteamVR is still running
            if (Processes.SteamVrProcess is { HasExited: true }) {
                await SteamVR.Exit();
            }
            
            if (Processes.VrChatProcess is { HasExited: true }) {
                VRCVideoCacher.Exit();
                AdGoBye.Exit();
                VRCX.Exit();
                Secret.SecretApp1.Exit();
            }
            
            IReadOnlyList<UserNotification> readOnlyListOfNotifications = _listener.GetNotificationsAsync(NotificationKinds.Toast).AsTask().Result;
            
            foreach (var userNotification in readOnlyListOfNotifications) {
                if (KnownNotifications.Contains(userNotification.Id)) continue;
                KnownNotifications.Add(userNotification.Id);

                try {
                    Windows.ApplicationModel.AppInfo? appInfo = null;
                    try { appInfo = userNotification.AppInfo; } catch { /*ignored*/ }
                    
                    var appName = appInfo != null ? appInfo.DisplayInfo.DisplayName : "";
                    var textElements = userNotification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric)?.GetTextElements();
                    var elementList = textElements?.Select(t => t.Text).ToArray();
                    if (elementList == null) continue;
                    var title = elementList?[0];
                    
                    switch (config.Whitelist) {
                        case true when !config.Applications!.Contains(appName!.ToLower()):
                        case false when config.Applications!.Contains(appName!.ToLower()):
                            continue;
                    }
                    
                    if (!TargetApplicationNames.Contains(appName!.ToLower())) return;

                    var text = elementList?.Length >= 2 ? string.Join("\n", elementList.Skip(1)) : "";
                    
                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(text)) continue;
                    
                    var height = CalculateHeight(text);
                    var timeout = CalculateTimeout(text);
                    var truncateText = height > 250f;

                    if (text.ToLower().ContainsMultiple(".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".tiff", ".tif", ".svg")) {
                        text = $"[image: {text}]";
                    } else if (text.Length == 0) {
                        text = "[sent an embed of some sort]";
                    }
                    
                    var xsNotification = new XSNotification {
                        Title = $"{appName} - {title}",               // supports Rich Text Formatting
                        Content = truncateText ? text[..1200] : text, // supports Rich Text Formatting
                        Timeout = timeout,                            // [float] seconds
                        SourceApp = Vars.AppName,                     // [string] name of your app
                        MessageType = XSMessageType.Notification,     // Notification or MediaPlayer
                        UseBase64Icon = false,                        // Base64 encoded image
                        Icon = "default",                             // Can also be "default", "error", or "warning"
                        Opacity = 0.8f,                               // [float] 0 to 1
                        Height = height,                              // [float] 0 to 250
                        Volume = 0.2f,                                // [float] 0 to 1
                        AudioPath = "default"                         // Can also be "default", "error", or "warning"
                    };
        
                    XsNotifier.SendNotification(xsNotification);
                    Logger.Information("Notification sent from {0}: \"{1} - {2}\"", appName, title, text);
#if DEBUG
                    Logger.Debug("JSON: {0}\n", xsNotification.AsJson());
#endif
                }
                catch (Exception e) {
                    Logger.Error(e, "Error sending notification.");
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private static int CalculateHeight(string content) =>
        content.Length switch {
            <= 100 => 100,
            <= 200 => 150,
            <= 300 => 200,
            _ => 250
        };
    
    private static float CalculateTimeout(string content) =>
        content.Length switch {
            <= 100 => 3f,
            <= 200 => 4f,
            <= 300 => 5f,
            _ => 6f
        };
}