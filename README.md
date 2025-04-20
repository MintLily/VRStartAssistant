<a name="readme-top"></a>

<div align="center">

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![MIT License][license-shield]][license-url]

</div>



<br />
<div align="center">
  <a href="https://github.com/MintLily/VRStartAssistant">
    <img src="Resources/banner.webp" alt="Banner Logo" height="200">
  </a>

  <h3 align="center">VRStartAssistant</h3>

  <p align="center">
    An all-in-one app that helps set up a one-click automated VR setup.
    <br />
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li><a href="#know-the-configuration">Know the Configuration</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

An all-in-one app that helps me set up a one-click automated VR setup. It starts various applications<sup>1</sup>, as well as, [SteamVR](https://store.steampowered.com/app/250820/SteamVR/), and [VRChat](https://hello.vrchat.com/) in a time-based, sequential order.
<br><br>
<small><sup>1</sup> - Based on what is in configuration file</small>

### Built With

[![Rider][Rider]][RiderUrl] [![VisualStudio][VisualStudio]][VisualStudioUrl] [![DotNet][CSharp]][DotNetUrl] [![Sublime Text][Sublime]][SublimeUrl]<br>
[![AudioSwitcher][AudioSwitcher]][AudioSwitcherUrl]<br>
[![RestSharp][RestSharp]][RestSharpUrl]<br>
[![Serilog][Serilog]][SerilogUrl] &nbsp;&nbsp;&nbsp; ![Serilog.Expressions] ![Serilog.Sinks.Console] ![Serilog.Sinks.File]<br>
[![XSNotifications][XSNotifications]][XSNotificationsUrl]<br>
[![CoreOSC][CoreOSC]][CoreOSCUrl]<br>
[![Spectre.Console][Spectre.Console]][Spectre.ConsoleUrl] &nbsp;&nbsp;&nbsp; ![Spectre.Console.Cli]

## Know the Configuration

<details>
  <summary>General</summary>
  <ul>
    <li><code>ConfigVersion</code> - Specifies the amount of times the configuration has been revised. <b>(DO NOT EDIT THIS VALUE)</b></li>
  </ul>
</details>

<details>
  <summary>VR</summary>
  <ul>
    <li><code>AutoLaunchWithSteamVr</code> - Allows you to set if you want this application to start automatically when you start SteamVR, instead of manually every time.</li>
    <li><code>HasRegistered</code> - Specifies whether SteamVR has been registered to auto start <b>(DO NOT EDIT THIS VALUE)</b></li>
  </ul>
</details>

<details>
  <summary>Audio</summary>
  <ul>
    <li><code>DefaultAudioDevice</code> - The ID number of the audio device</li>
    <li><code>ApplyAllDevicesToList</code> - Add all your speaker devices to the list below</li>
    <li>
      <code>AudioDevices</code> - The list of audio devices (speakers) you have the program auto switch to when starting the program / SteamVR
      <ul>
        <li>
          <details>
            <summary>Audio Entry</summary>
            <ul>
              <li><code>Id</code> - Number of ID</li>
              <li><code>Name</code> - Human Readable Name</li>
              <li><code>Guid</code> - Unique id for windows to assign</li>
            </ul>
          </details>
        </li>
      </ul>
    </li>
    <li><code>SwitchBackAudioDevice</code> - The device you want to switch back to after SteamVR closes</li>
  </ul>
</details>

<details>
  <summary>Home Assistant</summary>
  <ul>
    <li><code>Host</code> - The URL/IP address of your server [ex. <code>http://192.168.1.101/</code>]</li>
    <li><code>Token</code> - Authentication Token [<a href="https://img.mili.lgbt/7zq2UiOTr7oOJ84cXX2.png" target="_blank">Where to go to create your token</a>]</li>
    <li><code>ToggleSwitchEntityIds</code> - Your list of simple toggle devices [<a href="https://www.home-assistant.io/docs/configuration/customizing-devices/" target="_blank">follow this guide to find and/or customize your entities</a>]</li>
    <li><code>ControlLights</code> - Specify whether or not you want this program to control your lights (true or false value)</li>
    <li><code>LightEntityIds</code> - Your list of light IDs [<a href="https://www.home-assistant.io/docs/configuration/customizing-devices/" target="_blank">follow this guide to find and/or customize your entities</a>]</li>
    <li><code>LightBrightness</code> - Value of brightness of lights [value from 0 to 100]</li>
    <li><code>LightColor</code> - R, G, B value of the lights</li>
  </ul>
</details>

<details>
  <summary>VRChat Music OSC (Chatbox)</summary>
  <ul>
    <li><code>ListeningPort</code> - The OSC Listening port</li>
    <li><code>SendingPort</code> - The OSC Sending port</li>
    <li><code>ShowMediaStatus</code> - In VRChat, display a message above your head when a song changes (true or false value)</li>
    <li><code>ForceStartMediaStatus</code> - Forcably run the media service in case VRChat is not detected (true or false value)</li>
    <li><code>CustomBlockWordsContains</code> - List of word(s) used to no show the message [ex. if you add the word "star", any song name or artist with that word will not display a message]</li>
    <li><code>CustomBlockWordsEquals</code> - List of word(s) used to no show the message [ex. if you add the word(s) "Tokyo Machine", and song or artist matching that will not display a message]</li>
    <li><code>SecondsToAutoHideChatBox</code> - Number of seconds the message will show for</li>
  </ul>
</details>

<details>
  <summary>Programs</summary>
  <ul>
    <li><code>Programs</code> - The list of programs you want to auto start with this program
      <ul>
        <li>
          <details>
            <summary>Program Entry</summary>
            <ul>
              <li><code>Name</code> - The name so you know what it is</li>
              <li><code>ExePath</code> - File path to the program excecutable</li>
              <li><code>Arugments</code> - Any arguments you may need for that program</li>
              <li><code>StartWithVrsa</code> - Specify if you want the your program to start with VRSA</li>
              <li><code>StartMinimized</code> - Start your program with it's window minimized</li>
              <li><code>HasMultiProcesses</code> - Specify with the program has multiple processes [ex. VRCX has multiple processes]</li>
              <li><code>RelaunchIfCrashed</code> - If true, VRSA will try to relaunch your program if it becomes closed</li>
              <li><code>ProcessName</code> - The name of the process [<a href="https://img.mili.lgbt/vTTATZ8TR2HV8o7O5e.png" target="_blank">find out how to get this name here (me selecting VRCX)</a>]</li>
              <li><code>FallbackProcessStartingNeeded</code> - Set this to true if the program does not launch your program correctly</li>
            </ul>
          </details>
        </li>
      </ul>
    </li>
  </ul>
</details>

<!-- LICENSE -->
## License

Distributed under the MIT License. See [`LICENSE`][license-url] for more information.<br>
Specific GPL-2.0 applies to [Paci](https://github.com/PaciStardust/HOSCY)'s code [in this file](https://github.com/MintLily/VRStartAssistant/blob/main/VRStartAssistant/Features/OscMedia.cs) (`OscMedia.cs`)<br>
Specific BSD applies to [tallesl](https://github.com/tallesl)'s code [in this folder](https://github.com/MintLily/VRStartAssistant/blob/main/VRStartAssistant/FluentScheduler) (`FluentScheduler/`)

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Othneil Drew - for the [README Style](https://github.com/othneildrew/Best-README-Template)
* Katie - For help with the Windows API (From [WindowsXSO][WindowsXSOUrl])
* [xenolightning](https://github.com/xenolightning) - For Audio Switching (from [AudioSwitcher](https://github.com/xenolightning/AudioSwitcher))
* [Home Assistant](https://www.home-assistant.io/) & HA Community - For their [RestAPI](https://developers.home-assistant.io/docs/api/rest/) docs
* * [Elly](https://ellyvr.dev/) - For sanity checking my first time use of websocket/api things in C# using [RestSharp](https://restsharp.dev/)
* [Paci](https://github.com/PaciStardust) - For parts of [HOSCY](https://github.com/PaciStardust/HOSCY)'s code for OSC Chatbox things
* [LucHeart](https://github.com/LucHeart) - For OSC help
* [tallesl](https://github.com/tallesl) - for their automated job scheduler, [FluentScheduler](https://github.com/fluentscheduler/FluentScheduler)
* * [SamuelDeCarvalho](https://github.com/SamuelDeCarvalho), [ExtraTNT](https://github.com/ExtraTNT), [rafis-tatar](https://github.com/rafis-tatar) - Forks with edits

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/MintLily/VRStartAssistant.svg?style=for-the-badge
[contributors-url]: https://github.com/MintLily/VRStartAssistant/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/MintLily/VRStartAssistant.svg?style=for-the-badge
[forks-url]: https://github.com/MintLily/VRStartAssistant/network/members
[stars-shield]: https://img.shields.io/github/stars/MintLily/VRStartAssistant.svg?style=for-the-badge
[stars-url]: https://github.com/MintLily/VRStartAssistant/stargazers
[issues-shield]: https://img.shields.io/github/issues/MintLily/VRStartAssistant.svg?style=for-the-badge
[issues-url]: https://github.com/MintLily/VRStartAssistant/issues
[license-shield]: https://img.shields.io/github/license/MintLily/VRStartAssistant.svg?style=for-the-badge
[license-url]: https://github.com/MintLily/VRStartAssistant/blob/main/LICENSE
[releases-url]: https://github.com/MintLily/VRStartAssistant/releases

[Rider]: https://img.shields.io/badge/Rider-000000?style=for-the-badge&logo=rider&logoColor=white
[RiderUrl]: https://jb.gg/OpenSourceSupport
[CSharp]: https://img.shields.io/badge/DotNet%209-512BD4?style=for-the-badge&logo=sharp&logoColor=white
[DotNetUrl]: https://dotnet.microsoft.com/en-us/download/dotnet/9.0
[Sublime]: https://img.shields.io/badge/Sublime%20Text-FF9800?style=for-the-badge&logo=sublimetext&logoColor=white
[SublimeUrl]: https://www.sublimetext.com/
[VisualStudio]: https://img.shields.io/badge/Visual%20Studio-463668?style=for-the-badge&logo=vs&logoColor=white
[VisualStudioURL]: https://visualstudio.microsoft.com/

<!-- NuGet Packages -->
[AudioSwitcher]: https://img.shields.io/badge/AudioSwitcher-004880?style=for-the-badge&logo=nuget&logoColor=white
[AudioSwitcherUrl]: https://www.nuget.org/packages/AudioSwitcher.AudioApi.CoreAudio
[RestSharp]: https://img.shields.io/badge/RestSharp-004880?style=for-the-badge&logo=nuget&logoColor=white
[RestSharpUrl]: https://www.nuget.org/packages/RestSharp
[Serilog]: https://img.shields.io/badge/Serilog-004880?style=for-the-badge&logo=nuget&logoColor=white
[SerilogUrl]: https://www.nuget.org/packages/Serilog/
[Serilog.Expressions]: https://img.shields.io/badge/Serilog.Expressions-005a80?style=for-the-badge&logo=nuget&logoColor=white
[Serilog.Sinks.Console]: https://img.shields.io/badge/Serilog.Sinks.Console-005a80?style=for-the-badge&logo=nuget&logoColor=white
[Serilog.Sinks.File]: https://img.shields.io/badge/Serilog.Sinks.File-005a80?style=for-the-badge&logo=nuget&logoColor=white
[XSNotifications]: https://img.shields.io/badge/XSNotifications-004880?style=for-the-badge&logo=nuget&logoColor=white
[XSNotificationsUrl]: https://www.nuget.org/packages/XSNotifications
[CoreOSC]: https://img.shields.io/badge/LucHeart.CoreOSC-004880?style=for-the-badge&logo=nuget&logoColor=white
[CoreOSCUrl]: https://www.nuget.org/packages/LucHeart.CoreOSC
[Spectre.Console]: https://img.shields.io/badge/Spectre.Console-004880?style=for-the-badge&logo=nuget&logoColor=white
[Spectre.ConsoleUrl]: https://www.nuget.org/packages/Spectre.Console
[Spectre.Console.Cli]: https://img.shields.io/badge/Spectre.Console.Cli-005a80?style=for-the-badge&logo=nuget&logoColor=white

<!-- Other Links -->
[XSOverlaySteam]: https://store.steampowered.com/app/1173510/XSOverlay/
[WindowsXSOUrl]: https://github.com/Minty-Labs/WindowsXSO
