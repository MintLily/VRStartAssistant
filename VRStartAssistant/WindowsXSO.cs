using System.Diagnostics;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using Serilog;
using XSNotifications;
using XSNotifications.Enum;

namespace VRStartAssistant; 

public class WindowsXSO {
    public WindowsXSO() => Log.Information("[{0}] Setting up {Name} :: {Description}", "MODULE", "WindowsXSO", "Windows to XSOverlay Notification Relay");

    private static UserNotificationListener? _listener;
    private static readonly List<uint> KnownNotifications = new List<uint>();
    private static readonly List<string> TargetApplicationNames = new() { "discord", "vesktop" };
    
    public async Task StartAsync() {
        _listener = UserNotificationListener.Current;
        var accessStatus = _listener.RequestAccessAsync().GetResults();

        var isInRestartMessage = false;
        switch (accessStatus) {
            case UserNotificationListenerAccessStatus.Allowed:
                Log.Information("[{0}] Notifications {1}.", "WINDOWSXSO", "access granted");
                break;
            case UserNotificationListenerAccessStatus.Denied:
                Log.Error("[{0}] Notifications {1}.", "WINDOWSXSO", "access denied");
                isInRestartMessage = true;
                Log.Warning("[{0}] Please grant access to notifications.", "WINDOWSXSO");
                Console.WriteLine("----------------------------------------");
                Log.Warning("<[{0}]>", "Windows 11");
                Log.Warning("(System) Settings > Privacy & Security > Notifications (Section) > Allow apps to access notifications > ON (true)");
                Log.Warning("<[{0}]>", "Windows 10");
                Log.Warning("(System) Settings > Notifications & actions > Get notifications from apps and other senders > ON (true)");
                Log.Warning("<[{0}]>", "BOTH");
                Log.Warning("Make sure Focus Assist is OFF (false)");
                Log.Warning("Once complete, restart this program.");
                Log.Warning($"Press any key to exit {Vars.AppName}.");
                Console.ReadKey();
                break;
            case UserNotificationListenerAccessStatus.Unspecified:
                Log.Warning("[{0}] Notifications {1}.", "WINDOWSXSO", "access unspecified");
                Log.Warning("[{0}] Notifications may not work as intended.", "WINDOWSXSO");
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        if (isInRestartMessage)
            Process.GetCurrentProcess().Kill();

        Log.Information("[{0}] Whitelist target applications: " + "{1}", "WINDOWSXSO", string.Join(", ", TargetApplicationNames!));

        Log.Information("[{0}] Starting notification listener...", "WINDOWSXSO");
        while (true) { // Keep the program running
            
            // Check if SteamVR is still running
            if (Processes.SteamVrProcess is { HasExited: true }) 
                await Program.SteamVrInstance.ExitApplicationWithSteamVr();
            
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
                    
                    if (!TargetApplicationNames.Contains(appName!.ToLower())) return;

                    var text = elementList?.Length >= 2 ? string.Join("\n", elementList.Skip(1)) : "";
                    
                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(text)) continue;
                    
                    var height = 175f;
                    var timeout = 6f;
                    
                    if (text.Length > 150) {
                        height += 25f;
                        timeout = 7f;
                    }

                    if (text.Length > 275) {
                        height += 75f;
                        timeout = 8f;
                    }

                    if (text.Length > 400) {
                        height += 155f;
                        timeout = 10f;
                    }

                    var truncateText = false;
                    if (text.Length > 500) {
                        height += 200f;
                        timeout = 12f;
                        truncateText = true;
                    }

                    if (text.ToLower().Contains("image.png") || text.ToLower().Contains("image.jpg") || text.ToLower().Contains("image.jpeg") || text.ToLower().Contains("unknown.png")) {
                        text = "Sent an image.";
                        height = 100f;
                        timeout = 3f;
                    }
                    
                    var xsNotification = new XSNotification {
                        Title = $"{appName} - {title}", // supports Rich Text Formatting
                        Content = truncateText ? text[..1500] : text, // supports Rich Text Formatting
                        Timeout = timeout, // [float] seconds
                        SourceApp = Vars.AppName,
                        MessageType = XSMessageType.Notification,
                        UseBase64Icon = false,
                        // Base64 encoded image
                        Icon = "default", // Can also be "default", "error", or "warning"
                        Opacity = 0.8f, // [float] 0 to 1
                        Height = height,
                        Volume = 0f, // [float] 0 to 1
                    };
        
                    new XSNotifier().SendNotification(xsNotification);
                    Log.Information("[{0}] Notification sent from {1}: \"{2} - {3}\"", "WINDOWSXSO", appName, title, text);
#if DEBUG
                    Log.Debug("[{0}] JSON: {1}\n", "WINDOWSXSO", xsNotification.AsJson());
#endif
                }
                catch (Exception e) {
                    Log.Error(e, "Error sending notification.");
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        // ReSharper disable once FunctionNeverReturns
    }
}