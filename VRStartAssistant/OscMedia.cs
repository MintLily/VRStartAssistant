using System.Net;
using System.Text;
using Windows.Media;
using Windows.Media.Control;
using LucHeart.CoreOSC;
using Serilog;

namespace VRStartAssistant;
// A lot of this came from https://github.com/PaciStardust/HOSCY :: Paci's Code Should follow their GPL-2.0 License
// as well as my girlfriend's private project

public class OscMedia {
    private static readonly ILogger Logger = Log.ForContext(typeof(OscMedia));
    private static GlobalSystemMediaTransportControlsSessionManager? _sessionManager;
    private static GlobalSystemMediaTransportControlsSession? _session;
    private static GlobalSystemMediaTransportControlsSessionMediaProperties? _nowPlaying;
    private static DateTime _mediaLastChanged = DateTime.MinValue;
    private static OscDuplex? _oscSender;
    private static string AddressGameTextbox { get; set; } = "/chatbox/input";

    private enum NotificationType {
        None,
        Counter,
        Media,
        Afk,
        External
    }

    internal static void StartMediaDetection() {
        _oscSender = new OscDuplex(
            new IPEndPoint(IPAddress.Loopback, Program.ConfigurationInstance.Base.OscThings.ListeningPort),
            new IPEndPoint(IPAddress.Loopback, Program.ConfigurationInstance.Base.OscThings.SendingPort)
            );
        Logger.Information("OSC Sender Started");
        StartMediaDetectionInternal().RunWithoutAwait();
    }
    
    internal static void StopMediaDetection() {
        _oscSender?.Dispose();
        Logger.Information("Stopped media detection service");
    }

    private static async Task StartMediaDetectionInternal() {
        Logger.Information("Started media detection service");

        _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        _sessionManager.CurrentSessionChanged += SessionManagerCurrentSessionChanged;

        GetCurrentSession(_sessionManager);
    }

    private static void GetCurrentSession(GlobalSystemMediaTransportControlsSessionManager sender) {
        _session = sender.GetCurrentSession();
        _nowPlaying = null;

        Logger.Information($"Media session has been changed to {_session?.SourceAppUserModelId ?? "None"}");

        if (_session == null)
            return;

        _session.MediaPropertiesChanged += Session_MediaPropertiesChanged;
        _session.PlaybackInfoChanged += Session_PlaybackInfoChanged;

        UpdateCurrentlyPlayingMediaProxy(_session);
    }

    private static readonly object Lock = new();

    private static async Task UpdateCurrentlyPlayingMedia(GlobalSystemMediaTransportControlsSession sender) {
        var newPlaying = await sender.TryGetMediaPropertiesAsync();
        var playbackInfo = sender.GetPlaybackInfo();

        // Set notification empty if the current media is invalid
        if (newPlaying == null // No new playing
            || newPlaying.PlaybackType is not (MediaPlaybackType.Video or MediaPlaybackType.Music) // Not a video or music
            || playbackInfo == null || playbackInfo.PlaybackStatus != GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing // No status or it is not playing
            || string.IsNullOrWhiteSpace(newPlaying.Title) // No title
            || (newPlaying.Title == "Up next" && newPlaying.Artist == "DJ X") // Is playing from AI DJ X
            || Program.ConfigurationInstance.Base.OscThings.CustomBlockWordsContains.Any(word => newPlaying.Title.Contains(word, StringComparison.OrdinalIgnoreCase)) // Is Title playing contains a blocked word
            || Program.ConfigurationInstance.Base.OscThings.CustomBlockWordsContains.Any(word => newPlaying.Artist.Contains(word, StringComparison.OrdinalIgnoreCase)) // Is Artist playing contains a blocked word
            || Program.ConfigurationInstance.Base.OscThings.CustomBlockWordsEquals.Any(word => newPlaying.Title.Equals(word, StringComparison.OrdinalIgnoreCase)) // Is Title playing equals a blocked word
            || Program.ConfigurationInstance.Base.OscThings.CustomBlockWordsEquals.Any(word => newPlaying.Artist.Equals(word, StringComparison.OrdinalIgnoreCase)) // Is Artist playing equals a blocked word
        ) {
            SetNotification(string.Empty);
            return;
        }

        // Checking if the new media is the same media
        // Locked as it sometimes happens to come here multiple times concurrently
        lock (Lock) {
            if (_nowPlaying != null) // We skip this check if there is now playing
                if (newPlaying.Title.Equals(_nowPlaying.Title, StringComparison.OrdinalIgnoreCase) && newPlaying.Artist.Equals(_nowPlaying.Artist) && (DateTime.Now - _mediaLastChanged).TotalSeconds < 5)
                    return;
        }

        _nowPlaying = newPlaying;
        _mediaLastChanged = DateTime.Now;

        if (Program.ConfigurationInstance.Base.OscThings.ShowMediaStatus) {
            var playing = CreateCurrentMediaString();

            if (string.IsNullOrWhiteSpace(playing)) {
                SetNotification(string.Empty);
                return;
            }

            // foreach (var filter in Config.Textbox.MediaFilters) {
            //     if (!filter.Enabled || !filter.Matches(playing)) continue;
            //     SetNotification(string.Empty);
            //     return;
            // }

            Logger.Information($"Currently playing media has changed to: {playing}");
            SetNotification($"{MediaPlayingVerb} {playing}");
        }
    }

    private static string MediaPlayingVerb { // xyz "songname" by "artist" on "album"
        get => _mediaPlayingVerb;
        set => _mediaPlayingVerb = value.Length > 0 ? value : "🎵";
    }

    private static string _mediaPlayingVerb = "🎵";

    private static string MediaArtistVerb { // Playing "songname" xyz "artist" on "album"
        get => _mediaArtistVerb;
        set => _mediaArtistVerb = value.Length > 0 ? value : "by";
    }

    private static string _mediaArtistVerb = "by";

    private static string MediaAlbumVerb { // Playing "songname" by "artist" xyz "album"
        get => _mediaAlbumVerb;
        set => _mediaAlbumVerb = value.Length > 0 ? value : "from";
    }

    private static string _mediaAlbumVerb = "on";

    private static string? CreateCurrentMediaString() {
        if (_nowPlaying == null || string.IsNullOrWhiteSpace(_nowPlaying.Title)) // This should in theory never trigger but just to be sure
            return null;

        StringBuilder sb = new();

        if (string.IsNullOrWhiteSpace(_nowPlaying.Artist))
            sb.Append($"'{_nowPlaying.Title}'");
        else {
            var appendText = $"'{_nowPlaying.Title}' {MediaArtistVerb} '{_nowPlaying.Artist}'";
            sb.Append(appendText);
        }

        // if (Config.Textbox.MediaAddAlbum && !string.IsNullOrWhiteSpace(_nowPlaying.AlbumTitle) && _nowPlaying.AlbumTitle != _nowPlaying.Title)
        //     sb.Append($" {Config.Textbox.MediaAlbumVerb} '{_nowPlaying.AlbumTitle}'");

        // if (!string.IsNullOrWhiteSpace(Config.Textbox.MediaExtra))
        //     sb.Append($" {Config.Textbox.MediaExtra}");

        return sb.ToString();
    }

    private static NotificationType _notificationType = NotificationType.None;
    private static string _notification = string.Empty;
    private static NotificationType _notificationTypeLast = NotificationType.None;

    private static void SetNotification(string input, NotificationType type = NotificationType.Media) {
        if (string.IsNullOrWhiteSpace(input)) {
            if (_notificationType == type)
                ClearNotification();
            return;
        }

        var indLen = NotificationIndicatorLength();
        input = input.Length > MaxLength - indLen ? input[..(MaxLength - indLen - 3)] + "..." : input;

        input = $"{NotificationIndicatorLeft}{input}{NotificationIndicatorRight}";

        _notificationType = type;
        _notification = input;
        // PageInfo.SetNotification(input, type);
        Program.ChangeConsoleTitle(input);
        Logger.Information("Setting notification to: " + input);
        Task.Run(async () => await SendOscMessage(AddressGameTextbox, [input, true, false]));
    }

    /// <summary>
    /// Clears the notification
    /// </summary>
    private static void ClearNotification() {
        _notification = string.Empty;
        _notificationTypeLast = _notificationType;
        _notificationType = NotificationType.None;
        Program.ChangeConsoleTitle();
    }

    private static string NotificationIndicatorLeft { // Text to the left of a notification
        get => _notificationIndicatorLeft;
        set {
            _notificationIndicatorLeft = value.Length < 4 ? value : value[..3];
            _notificationIndicatorLength = CalcNotificationIndicatorLength();
        }
    }

    private static string _notificationIndicatorLeft = "";

    private static string NotificationIndicatorRight { // Text to the right of a notification
        get => _notificationIndicatorRight;
        set {
            _notificationIndicatorRight = value.Length < 4 ? value : value[..3];
            _notificationIndicatorLength = CalcNotificationIndicatorLength();
        }
    }

    private static string _notificationIndicatorRight = "";
    private static int _notificationIndicatorLength = 2;

    private static int CalcNotificationIndicatorLength()
        => _notificationIndicatorRight.Length + _notificationIndicatorLeft.Length;

    private static int NotificationIndicatorLength() => _notificationIndicatorLength;

    private static int MaxLength { // Max length of string displayed before cutoff
        get => _maxLength;
        set => _maxLength = Utils.MinMax(value, 50, 130);
    }

    private static int _maxLength = 130;

    private static int TimeoutMultiplier { // Add x milliseconds to timeout per 20 characters
        get => _timeoutMultiplier;
        set => _timeoutMultiplier = Utils.MinMax(value, 250, 10000);
    }

    private static int _timeoutMultiplier = 1250;

    /// <summary>
    /// Triggers whenever is paused, skipped, etc
    /// </summary>
    private static void Session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        => UpdateCurrentlyPlayingMediaProxy(sender);

    /// <summary>
    /// Triggers on song switch
    /// </summary>
    private static void Session_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        => UpdateCurrentlyPlayingMediaProxy(sender);

    /// <summary>
    /// Triggers when a new session is detected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void SessionManagerCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        => GetCurrentSession(sender);

    private static void UpdateCurrentlyPlayingMediaProxy(GlobalSystemMediaTransportControlsSession sender)
        => UpdateCurrentlyPlayingMedia(sender).RunWithoutAwait();

    /// <summary>
    /// Enum to classify the different commands that can be executed
    /// </summary>
    private enum MediaCommandType {
        None,
        Pause,
        Unpause,
        Rewind,
        Skip,
        Info,
        TogglePlayback
    }

    /// <summary>
    /// List of command aliases
    /// </summary>
    private static readonly IReadOnlyDictionary<string, MediaCommandType> _commandTriggers = new Dictionary<string, MediaCommandType>() {
        { "pause", MediaCommandType.Pause },
        { "stop", MediaCommandType.Pause },

        { "resume", MediaCommandType.Unpause },
        { "play", MediaCommandType.Unpause },

        { "skip", MediaCommandType.Skip },
        { "next", MediaCommandType.Skip },

        { "rewind", MediaCommandType.Rewind },
        { "back", MediaCommandType.Rewind },

        { "info", MediaCommandType.Info },
        { "current", MediaCommandType.Info },
        { "status", MediaCommandType.Info },
        { "now", MediaCommandType.Info },

        { "toggle", MediaCommandType.TogglePlayback },
    };

    /// <summary>
    /// Handles the lowercase raw media command, skips otherwise
    /// </summary>
    /// <param name="command">Raw command</param>
    internal static bool HandleRawMediaCommand(string command) {
        command = command.Replace("media ", string.Empty);

        if (!_commandTriggers.Keys.Contains(command))
            return false;

        var mediaCommand = _commandTriggers[command];
        HandleMediaCommand(mediaCommand).RunWithoutAwait();
        return true;
    }

    /*/// <summary>
    /// Handles osc media commands
    /// </summary>
    /// <param name="address">Osc Address</param>
    /// <returns>Was a command executed</returns>
    internal static bool HandleOscMediaCommands(string address) {
        //I wish this could be a switch but those expect constants
        var command = MediaCommandType.None;

        if (address == Config.Osc.AddressMediaInfo)
            command = MediaCommandType.Info;
        else if (address == Config.Osc.AddressMediaToggle)
            command = MediaCommandType.TogglePlayback;
        else if (address == Config.Osc.AddressMediaPause)
            command = MediaCommandType.Pause;
        else if (address == Config.Osc.AddressMediaRewind)
            command = MediaCommandType.Rewind;
        else if (address == Config.Osc.AddressMediaSkip)
            command = MediaCommandType.Skip;
        else if (address == Config.Osc.AddressMediaUnpause)
            command = MediaCommandType.Unpause;

        HandleMediaCommand(command).RunWithoutAwait();
        return command != MediaCommandType.None;
    }*/

    /// <summary>
    /// Actual handling of media commands
    /// </summary>
    /// <param name="command">command type</param>
    private static async Task HandleMediaCommand(MediaCommandType command) {
        if (_session == null)
            return;

        switch (command) {
            case MediaCommandType.TogglePlayback:
                if (await _session.TryTogglePlayPauseAsync())
                    Logger.Information("Toggled media playback");
                return;

            case MediaCommandType.Pause:
                if (await _session.TryPauseAsync())
                    Logger.Information("Paused media playback");
                return;

            case MediaCommandType.Unpause:
                if (await _session.TryPlayAsync())
                    Logger.Information("Resumed media playback");
                return;

            case MediaCommandType.Skip:
                if (await _session.TrySkipNextAsync())
                    Logger.Information("Skipped media playback");
                return;

            case MediaCommandType.Rewind:
                if (await _session.TrySkipPreviousAsync())
                    Logger.Information("Rewinded media playback");
                return;

            case MediaCommandType.Info:
                var playing = CreateCurrentMediaString();
                if (string.IsNullOrWhiteSpace(playing))
                    return;
                SetNotification($"{MediaPlayingVerb} {playing}");
                return;

            default: return;
        }
    }

    private static async Task SendOscMessage(string address, params object[] args) {
        var msg = new OscMessage(address, args);
        await _oscSender.SendAsync(msg);
        await Task.Delay(TimeSpan.FromSeconds(Program.ConfigurationInstance.Base.OscThings.SecondToAutoHideChatBox));
        await _oscSender.SendAsync(new OscMessage(AddressGameTextbox, string.Empty));
    }
}