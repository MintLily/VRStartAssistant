<a name="readme-top"></a>

<div align="center">

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![MIT License][license-shield]][license-url]

</div>



<br />
<div align="center">
  <a href="https://github.com/Minty-Labs/WindowsXSO">
    <img src="Resources/banner.webp" alt="Banner Logo" height="200">
  </a>

  <h3 align="center">VRStartAssistant</h3>

  <p align="center">
    An all-in-one app that helps me set up a one-click automated VR setup.<br />
    <i>This is not meant for others to use out of the box. &mdash; no executables will be provided</i>
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
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

An all-in-one app that helps me set up a one-click automated VR setup. It starts [VRCX](https://github.com/vrcx-team/VRCX), [VRChat OSC Router](https://github.com/SutekhVRC/VOR), [SteamVR](https://store.steampowered.com/app/250820/SteamVR/), [AdGoBye](https://github.com/AdGoBye/AdGoBye), [VRChat](https://hello.vrchat.com/), [VRCVideoCacher](https://git.ellyvr.dev/Elly/VRCVideoCacher), [HOSCY](https://github.com/PaciStardust/HOSCY), [HeartRateOnStream-OSC](https://github.com/Curtis-VL/HeartRateOnStream-OSC), [OSCLeash](https://github.com/ZenithVal/OSCLeash), and (a custom/beta version of) [WindowsXSO][WindowsXSOUrl] in a time-based, sequential order.
<br>
It also automatically turns on some smart plugs used for Base Stations, and turns them off upon SteamVR exit.

### Built With

[![Rider][Rider]][RiderUrl] [![DotNet][CSharp]][DotNetUrl] [![Sublime Text][Sublime]][SublimeUrl]<br>
[![AudioSwitcher][AudioSwitcher]][AudioSwitcherUrl]<br>
[![RestSharp][RestSharp]][RestSharpUrl]<br>
[![Serilog][Serilog]][SerilogUrl] &nbsp;&nbsp;&nbsp; ![Serilog.Expressions] ![Serilog.Sinks.Console] ![Serilog.Sinks.File]<br>
[![XSNotifications][XSNotifications]][XSNotificationsUrl]<br>
[![CoreOSC][CoreOSC]][CoreOSCUrl]

<!-- LICENSE -->
## License

Distributed under the MIT License. See [`LICENSE`][license-url] for more information.

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Othneil Drew - for the [README Style](https://github.com/othneildrew/Best-README-Template)
* Katie - For help with the Windows API (From [WindowsXSO][WindowsXSOUrl])
* [xenolightning](https://github.com/xenolightning) - For Audio Switching (from [AudioSwitcher](https://github.com/xenolightning/AudioSwitcher))
* [Home Assistant](https://www.home-assistant.io/) & HA Community - For their [RestAPI](https://developers.home-assistant.io/docs/api/rest/) docs
* * [Elly](https://github.com/Ellyvr) ([GitLab](https://git.ellyvr.dev/Elly)) - For sanity checking my first time use of websocket/api things in C# using [RestSharp](https://restsharp.dev/)
* [Paci](https://github.com/PaciStardust) - For parts of [HOSCY](https://github.com/PaciStardust/HOSCY)'s code for OSC Chatbox things
* [LucHeart](https://github.com/LucHeart) - For OSC help

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
[CSharp]: https://img.shields.io/badge/DotNet%208-512BD4?style=for-the-badge&logo=csharp&logoColor=white
[DotNetUrl]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[Sublime]: https://img.shields.io/badge/Sublime%20Text-FF9800?style=for-the-badge&logo=sublimetext&logoColor=white
[SublimeUrl]: https://www.sublimetext.com/

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

<!-- Other Links -->
[XSOverlaySteam]: https://store.steampowered.com/app/1173510/XSOverlay/
[WindowsXSOUrl]: https://github.com/Minty-Labs/WindowsXSO
