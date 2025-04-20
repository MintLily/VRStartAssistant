using System.Text;
using Serilog;
using Spectre.Console;
#if DEBUG
using VRStartAssistant.Configuration.debug;
#endif
using VRStartAssistant.Features;
using VRStartAssistant.Features.Integrations;
using FluentScheduler;
using VRStartAssistant.Features.Apps;
using VRStartAssistant.Utils;
using static VRStartAssistant.Utils.Keybinds.KeystrokeActions;
using static VRStartAssistant.Utils.StringUtils;

namespace VRStartAssistant;

public class MainWindow {
    internal static MainWindow Instance { get; private set; }
    public MainWindow() => Instance = this;

    private List<string> _consoleOutput = [];
    private bool _showingMainConsoleOutput;

    //private static readonly ILogger Logger = Log.ForContext<MainWindow>();
    private bool _showingMainGui = true,
        _showingConfigChangeTextGui,
        _isChangingAudioDevice,
        _isChangingHassHost,
        _isChangingHassToken,
        _isChangingMediaStatusTimeout,
        _isChangingOscListeningPort,
        _isChangingOscSendingPort,
        
        _hasStarted;

    private string _configChangePromptTitle, _configChangePromptValue;
    private List<string> _configChangePromptStringChoices = [];
    private List<int> _configChangePromptIntChoices = [];

    private void ResetValues() {
        _showingMainGui = true;
        _showingConfigChangeTextGui = false;
        _isChangingAudioDevice = false;
        _isChangingHassHost = false;
        _isChangingHassToken = false;
        _isChangingOscListeningPort = false;
        _isChangingOscSendingPort = false;
        _isChangingMediaStatusTimeout = false;
        _configChangePromptTitle = string.Empty;
        _configChangePromptValue = string.Empty;
        _configChangePromptStringChoices = [];
        _configChangePromptIntChoices = [];
    }

    internal void UpdateConsoleOutput(string? message, Serilog.Events.LogEventLevel logLevel = Serilog.Events.LogEventLevel.Information) {
        _consoleOutput ??= [];
        _consoleOutput.Insert(0, message ?? "null_or_no_message");
#pragma warning disable CA2254
        Program.MainLogger.Write(logLevel, message ?? "null_or_no_message");
#pragma warning restore CA2254
    }

    internal async Task Gui() {
        var config = Program.ConfigurationInstance!.Base!;
        AudioSwitch.GetAudioDevices();
        
        if (Vars.LaunchOption_PrintAudioDevices) {
            var tempLogger = Log.ForContext<AudioSwitch>();
            foreach (var device in config.Audio.AudioDevices) {
                tempLogger.Information("Device Name: {Name} - Device ID: {Id}", device.Name, device.Id);
            }
            tempLogger.Information("Audio Devices Count: {Count}", config.Audio.AudioDevices.Count);
            tempLogger.Information("Press any key to to resume application startup...");
            Console.ReadKey();
        }

        if (Vars.LaunchOption_GetAudioDevices) {
            AudioSwitch.GetAudioDevices(true);
        }
        
        _showingMainConsoleOutput = true;
        
        #region Key Bind Actions
        
        RegisterKeyCombination(ConsoleKey.Q, ctrl: true, action: ResetValues);
        
        RegisterKeyCombination(ConsoleKey.Add, () => { _consoleOutput.Insert(0, "Line Test"); });
        
#if DEBUG
        RegisterKeyCombinationAsync(ConsoleKey.T, async () => {
            Program.DebugConfigurationInstance.Base.RunApps = !Program.DebugConfigurationInstance.Base.RunApps;
            Program.DebugConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]DEBUG[/]]] " + (Program.DebugConfigurationInstance.Base.RunApps ? "[darkseagreen3_1]Enabled[/] " : "[plum3]Disabled[/] ") + "running applications");
            if (!Program.DebugConfigurationInstance.Base.RunApps) return;
            await HASS.ToggleBaseStations(toggleOff: false);
            await Program.StartApplications();
        }, ctrl: true);
#endif
        
        RegisterKeyCombination(ConsoleKey.D1, () => {
            ResetValues();
            _showingMainGui = false;
            _showingConfigChangeTextGui = true;
            _isChangingAudioDevice = true;
            _configChangePromptTitle = "Select your preferred Audio Device [italic dim](Enter the number of the entry above)[/]";

            foreach (var device in config.Audio.AudioDevices) {
                _configChangePromptIntChoices?.Add(device.Id);
            }
        }, ctrl: true);
        
        RegisterKeyCombination(ConsoleKey.D2, () => {
            var temp = !config.VR.HasRegistered;
            config.VR.HasRegistered = temp;
            Program.ConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]SteamVR[/]]] " + ((temp ? "[darkseagreen3_1]Registered[/]" : "[plum3]Unregistered[/]") + " to SteamVR. [dim]Changes will take affect next time you start this program.[/]"));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D3, () => {
            ResetValues();
            _showingMainGui = false;
            _showingConfigChangeTextGui = true;
            _isChangingHassHost = true;
            _configChangePromptTitle = "Enter your Home Assistant Host [italic dim](Enter the Server IP or URL address)[/]:";
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D3, () => {
            ResetValues();
            _showingMainGui = false;
            _showingConfigChangeTextGui = true;
            _isChangingHassToken = true;
            _configChangePromptTitle = "Enter your Home Assistant Token [italic dim](Enter the token)[/]:";
        }, alt: true);

        RegisterKeyCombination(ConsoleKey.D4, () => {
            var temp = !config.HASS.ControlSwitches;
            config.HASS.ControlSwitches = temp;
            Program.ConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]Home Assistant[/]]] " + ((temp ? "[darkseagreen3_1]Enabled[/]" : "[plum3]Disabled[/]") + " Switch Controls. [dim]Changes will take affect next time you start this program.[/]"));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D5, () => {
            var temp = !config.HASS.ControlLights;
            config.HASS.ControlLights = temp;
            Program.ConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]Home Assistant[/]]] " + ((temp ? "[darkseagreen3_1]Enabled[/]" : "[plum3]Disabled[/]") + " Light Controls. [dim]Changes will take affect next time you start this program.[/]"));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D6, () => {
            ResetValues();
            _showingMainGui = false;
            _showingConfigChangeTextGui = true;
            _isChangingOscListeningPort = true;
            _configChangePromptTitle = "Enter the OSC Listening Port [italic dim](Enter the port number)[/]:";
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D6, () => {
            ResetValues();
            _showingMainGui = false;
            _showingConfigChangeTextGui = true;
            _isChangingOscSendingPort = true;
            _configChangePromptTitle = "Enter the OSC Sending Port [italic dim](Enter the port number)[/]:";
        }, alt: true);

        RegisterKeyCombination(ConsoleKey.D7, () => {
            var temp = !config.OscMusic.ShowMediaStatus;
            config.OscMusic.ShowMediaStatus = temp;
            Program.ConfigurationInstance.Save();
            OscMedia.HasMediaDetectionRunning = temp;
            OscMedia.ToggleMediaDetection();
            UpdateConsoleOutput("[[[gold1]OSC Media[/]]] " + ((temp ? "[darkseagreen3_1]Enabled[/]" : "[plum3]Disabled[/]") + " Media Status"));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.A, () => {
            var temp = config.OscMusic.SecondsToAutoHideChatBox + 1;
            config.OscMusic.SecondsToAutoHideChatBox = temp;
            Program.ConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]OSC Media[/]]] " + ("Chat Box Timeout set to [darkseagreen3_1]" + temp + "[/] seconds"));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.D, () => {
            var temp = config.OscMusic.SecondsToAutoHideChatBox - 1;
            if (temp <= 1)
                temp = 1;

            var seconds = "second";
            if (temp != 1)
                seconds += "s";

            config.OscMusic.SecondsToAutoHideChatBox = temp;
            Program.ConfigurationInstance.Save();
            UpdateConsoleOutput("[[[gold1]OSC Media[/]]] " + ("Chat Box Timeout set to [darkseagreen3_1]" + temp + "[/] "+ seconds));
            ResetValues();
        }, ctrl: true);

        RegisterKeyCombination(ConsoleKey.R, () => {
            Program.ConfigurationInstance.Load();
            UpdateConsoleOutput("[[[gold1]Configuration[/]]] Reloaded Configuration");
        }, ctrl: true);

#endregion

        Console.Clear();
        while (true) {
            Console.SetCursorPosition(0, 0);
            if (Console.KeyAvailable) { // Check for key release
                var keyInfo = Console.ReadKey(intercept: true);
                await OnKeyReleaseAsync(keyInfo);
            }

            if (_showingMainGui) {
                AnsiConsole.Write(new Rule($"[bold lightslateblue]{Vars.AppName}[/] v[hotpink3_1]{Vars.AppVersion}[/]"
#if DEBUG
                                           + $" - Development Build 29 - Apps Active: {(Program.DebugConfigurationInstance.Base.RunApps ? "[darkseagreen3_1]TRUE[/]" : "[plum3]FALSE[/]")}"
#endif
                                           ).Centered());

                var configDisplayStringBuilder = new StringBuilder();
                configDisplayStringBuilder.AppendLine($"Audio Device: [mistyrose1]{config.Audio.AudioDevices[config.Audio.DefaultAudioDevice].Name}[/]");
                configDisplayStringBuilder.AppendLine($"Registered to SteamVR: {(config.VR.HasRegistered ? "[darkseagreen3_1]True[/]" : "[plum3]False[/]")}");
                configDisplayStringBuilder.AppendLine("Home Assistant");
                configDisplayStringBuilder.AppendLine($"    Host: [underline mistyrose1]{config.HASS.Host}[/]");
                configDisplayStringBuilder.AppendLine($"    Control [mistyrose1]{config.HASS.ToggleSwitchEntityIds.Count}[/] Switches: {(config.HASS.ControlSwitches ? "[darkseagreen3_1]True[/]" : "[plum3]False[/]")}");
                configDisplayStringBuilder.AppendLine($"    Control [mistyrose1]{config.HASS.LightEntityIds.Count}[/] Lights: {(config.HASS.ControlLights ? "[darkseagreen3_1]True[/]" : "[plum3]False[/]")}");
                configDisplayStringBuilder.AppendLine("OSC Music Text Box [italic dim](VRChat)[/]");
                configDisplayStringBuilder.AppendLine($"    Listening Port: [mistyrose1]{config.OscMusic.ListeningPort}[/]");
                configDisplayStringBuilder.AppendLine($"    Sending Port: [mistyrose1]{config.OscMusic.SendingPort}[/]");
                configDisplayStringBuilder.AppendLine($"    Show Media Status: {(config.OscMusic.ShowMediaStatus ? "[darkseagreen3_1]True[/]" : "[plum3]False[/]")}");
                configDisplayStringBuilder.AppendLine($"    OSC Box Timeout [italic](seconds)[/]: [mistyrose1]{config.OscMusic.SecondsToAutoHideChatBox}[/]");
                configDisplayStringBuilder.AppendLine($"    Blocked Words/Phrases: [mistyrose1]{config.OscMusic.CustomBlockWordsContains.Count + config.OscMusic.CustomBlockWordsEquals.Count}[/]");
                configDisplayStringBuilder.AppendLine("Programs");
                var coloredProgramList = config.Programs.Select(program => program.StartWithVrsa ? $"[darkseagreen3_1]{program.Name}[/]" : $"[plum3]{program.Name}[/]").ToList();
                configDisplayStringBuilder.Append($"    Names: [mistyrose1]{string.Join(", ", coloredProgramList)}[/]");

                var configPanel = new Panel(configDisplayStringBuilder.ToString()) {
                    Header = new PanelHeader("[lightgoldenrod2_2]Configuration[/]"),
                    Expand = false,
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0)
                };

                var actionsStringBuilder = new StringBuilder();
                actionsStringBuilder.AppendLine("[italic dim](Numbers are on the top row of the keyboard)[/]");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 1[/] :: Set Audio Device");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 2[/] :: Register application to SteamVR");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 3[/] :: Set Home Assistant Host");
                actionsStringBuilder.AppendLine("[lightcoral]  ALT + 3[/] :: Set Home Assistant Token");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 4[/] :: Toggle Switches");
                // actionsStringBuilder.AppendLine("[lightcoral]  ALT + 4[/] :: Change switch entity IDs");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 5[/] :: Toggle Lights");
                // actionsStringBuilder.AppendLine("[lightcoral]  ALT + 5[/] :: Change light entity IDs");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 6[/] :: Set OSC Listening Port");
                actionsStringBuilder.AppendLine("[lightcoral]  ALT + 6[/] :: Set OSC Sending Port");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + 7[/] :: Toggle Show Media Status");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + A[/] :: Add Seconds to Auto Hide Chat Box");
                actionsStringBuilder.AppendLine("[lightcoral] CTRL + D[/] :: Subtract Seconds to Auto Hide Chat Box");
                if (Updater.Updater.IsUpdateAvailable) {
                    actionsStringBuilder.AppendLine("[lightgreen]       F1[/] :: [lightgreen]UPDATE APPLICATION[/]");
                }
                
#if DEBUG
                actionsStringBuilder.AppendLine("[gold1] CTRL + T[/] :: [gold1]Toggle Development Mode Run Apps[/]");
#endif
                actionsStringBuilder.Append(" [bold underline red]CTRL + C[/] :: [italic red]Exit Application[/]");

                var actionPanel = new Panel(actionsStringBuilder.ToString()) {
                    Header = new PanelHeader("[lightgoldenrod2_2]Actions[/] ([gold1]Keybinds[/])"),
                    Expand = true,
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0, 1, 0)
                };

                var consoleOutputPanel = new Panel(string.Join(Environment.NewLine, _consoleOutput.ShowLines(config.ConsoleOutputMaxLines))) {
                    Header = new PanelHeader("[lightgoldenrod2_2]Logs[/]"),
                    Expand = true,
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1)
                };

                var layout = new Grid()
                    .AddColumn()
                    .AddColumn()
                    .AddRow(configPanel, actionPanel);

                AnsiConsole.Write(layout);
                if (_showingMainConsoleOutput) {
                    AnsiConsole.Write(consoleOutputPanel);
                }
            }

            if (!_hasStarted) {
#if DEBUG
                if (Program.DebugConfigurationInstance!.Base!.RunApps) {
#endif
                    await HASS.ToggleBaseStations(toggleOff: false);
                    await Program.StartApplications();
                    
                    new Schedule(
                        Program.ConfigurationInstance.MidLoadConfigChanges,
                        run => run.Every(5).Minutes()
                    ).Start();
                    new Schedule(
                        () => {
                            Try.Catch(async () => {
                                await ProgramManager.RelaunchAppIfCrashed();
                            });
                        },
                        run => run.Every(5).Minutes()
                    ).Start();
                    _hasStarted = true;
#if DEBUG
                }
#endif
            }

            if (_showingConfigChangeTextGui) {
                Console.Clear();AnsiConsole.Write(new Rule($"[bold lightslateblue]{Vars.AppName}[/] v[hotpink3_1]{Vars.AppVersion}[/]"
#if DEBUG
                                                           + $"{(!Vars.IsDebug ? "" : 
                                                               $" - Development Build - Apps Active: {(Program.DebugConfigurationInstance.Base.RunApps ? "[darkseagreen3_1]TRUE[/]" : "[plum3]FALSE[/]")}")}"
#endif
                                                           ).Centered());
                AnsiConsole.WriteLine("Press \"CTRL + Q\" in case this menu gets stuck");
                AnsiConsole.WriteLine();

#region Int Choice Prompt

                if (_isChangingAudioDevice) {
                    if (_configChangePromptIntChoices is null) {
                        _consoleOutput.Insert(0, "<[red]ERROR[/]> [red]INPUT CHOICES WERE null![/]");
                        ResetValues();
                    }
                    else if (_configChangePromptIntChoices.Count() != 0) {
                        AnsiConsole.WriteLine("Choices:");
                        var list = new List<string>();

                        if (_isChangingAudioDevice) {
                            list = config.Audio.AudioDevices.Select(device => device.Name).ToList();
                        }

                        for (var i = 0; i < list.Count; i++) {
                            AnsiConsole.WriteLine($"{i} - {list[i]}");
                        }

                        AnsiConsole.WriteLine();

                        _configChangePromptValue = AnsiConsole.Prompt(
                            new TextPrompt<string>(_configChangePromptTitle)
                                .AddChoices(_configChangePromptIntChoices.Select(i => i.ToString())));
                    }
                }

#endregion

#region String Choice Prompt

                if (true) {
                    if (_configChangePromptStringChoices is null) {
                        _consoleOutput.Insert(0, "<[red]ERROR[/]> [red]INPUT CHOICES WERE null![/]");
                        ResetValues();
                    }
                    else if (_configChangePromptStringChoices.Count() != 0) {
                        AnsiConsole.WriteLine("Choices:");
                        foreach (var choice in _configChangePromptStringChoices) {
                            AnsiConsole.WriteLine($"[mistyrose1]{choice}[/]");
                        }

                        AnsiConsole.WriteLine();
                        _configChangePromptValue = AnsiConsole.Prompt(
                            new TextPrompt<string>(_configChangePromptTitle)
                                .AddChoices(_configChangePromptStringChoices));
                    }
                }

#endregion

#region String Prompt

                if (_isChangingHassHost) {
                    AnsiConsole.WriteLine();
                    _configChangePromptValue = AnsiConsole.Prompt(
                        new TextPrompt<string>(_configChangePromptTitle));
                }

                if (_isChangingHassToken) {
                    AnsiConsole.WriteLine();
                    _configChangePromptValue = AnsiConsole.Prompt(
                        new TextPrompt<string>(_configChangePromptTitle).Secret());
                }
                
                if (_isChangingOscListeningPort || _isChangingOscSendingPort || _isChangingMediaStatusTimeout) {
                    AnsiConsole.WriteLine();
                    _configChangePromptValue = AnsiConsole.Prompt(
                        new TextPrompt<string>(_configChangePromptTitle)
                            .AllowEmpty()
                            .Validate(input => input?.AsInt() != null ? ValidationResult.Success() : ValidationResult.Error("Please enter a valid number"))).ToString();
                }

#endregion
                
                if (!string.IsNullOrWhiteSpace(_configChangePromptValue)) {
                    if (_isChangingAudioDevice) {
                        var selectedAudioDevice = config.Audio.AudioDevices.FirstOrDefault(device => device.Id == _configChangePromptValue.AsInt());
                        if (selectedAudioDevice is not null) {
                            config.Audio.DefaultAudioDevice = selectedAudioDevice.Id;
                            Program.ConfigurationInstance.Save();
                            UpdateConsoleOutput($"[[[gold1]OSC Media[/]]] [darkseagreen3_1]Audio Device[/] set to [mistyrose1]{selectedAudioDevice.Name}[/]");
                            ResetValues();
                        }
                    }

                    if (_isChangingHassHost) {
                        if (_configChangePromptValue is "_back") {
                            ResetValues();
                            continue;
                        }

                        config.HASS.Host = _configChangePromptValue;
                        Program.ConfigurationInstance.Save();
                        UpdateConsoleOutput($"[[[gold1]Home Assistant[/]]] [darkseagreen3_1]Home Assistant Host[/] set to [mistyrose1]{_configChangePromptValue}[/], this will take affect on next launch");
                        ResetValues();
                    }

                    if (_isChangingHassToken) {
                        if (_configChangePromptValue is "_back") {
                            ResetValues();
                            continue;
                        }

                        config.HASS.Token = _configChangePromptValue;
                        Program.ConfigurationInstance.Save();
                        UpdateConsoleOutput($"[[[gold1]Home Assistant[/]]] [darkseagreen3_1]Home Assistant Token[/] set to [mistyrose1]{_configChangePromptValue.SimpleRedact()}[/], this will take affect on next launch");
                        ResetValues();
                    }
                    
                    if (_isChangingOscListeningPort) {
                        config.OscMusic.ListeningPort = _configChangePromptValue.AsInt();
                        Program.ConfigurationInstance.Save();
                        await OscMedia.RestartMediaDetection();
                        UpdateConsoleOutput($"[[[gold1]OSC Media[/]]] [darkseagreen3_1]OSC Listening Port[/] set to [mistyrose1]{_configChangePromptValue}[/]");
                        ResetValues();
                    }
                    
                    if (_isChangingOscSendingPort) {
                        config.OscMusic.SendingPort = _configChangePromptValue.AsInt();
                        Program.ConfigurationInstance.Save();
                        await OscMedia.RestartMediaDetection();
                        UpdateConsoleOutput($"[[[gold1]OSC Media[/]]] [darkseagreen3_1]OSC Sending Port[/] set to [mistyrose1]{_configChangePromptValue}[/]");
                        ResetValues();
                    }
                }
            }
            
            await Task.Delay(100);
            
            // Run the rest of the program
            if (Processes.SteamVrProcess is not { HasExited: true }) continue;
            await Program.CheckForExitedProcess();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}