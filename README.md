# ModernWpfCommunityToolkit

The **ModernWpf Community Toolkit** is a collection of helper functions and custom controls for the **[ModernWpf](https://github.com/Kinnara/ModernWpf)** library.

This project incorporates components from the [Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit).
The [Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit)'s license and 3rd party notices could be found [here](https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/master/license.md) and [here](https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/master/ThirdPartyNotices.txt) respectively.

## ModernWpf Community Toolkit & the [Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit)

First of all, we must thank the contributors of the **WCT** for their awesome work on the toolkit.

The **ModernWpf Community Toolkit (MCT)** is a port of the well-known & popular **[Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit)** to **WPF**/**ModernWpf**.

The **[Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit) (WCT)** [being a part of the **.NET Foundation** and actively developed by **Microsoft** and the **community**] is a collection of helper functions, custom controls and app services which simplifies and helps the development of UWP apps for Windows.

It has some parts which depend on some **UWP only APIs** (which can't be used from Win32 apps). And some parts are dependent on the **UWP XAML** (aka **WinUI**). Thus, rendering the WCT unusable from some application (such as **pure WPF apps**).

To resolve this barrier, **ModernWpf Community Toolkit** was made.

**ModernWpf Community Toolkit** is also a collection of helper functions and custom controls but for building **WPF (Win32) apps**.

This toolkit depends on another well-known & popular library called **[ModernWpf](https://github.com/Kinnara/ModernWpf)** which brings **WinUI** controls & styles to **WPF** applications. Whereas, this toolkit brings some **WCT** controls & helpers to **WPF**.

There may be some differences (in APIs or availability of some features) between **MCT** and **WCT** due to certain difference between WPF (.NET) & UWP (WinRT) APIs.

Most of the source code are ported from the **WCT** (some differences or variations can be found).

This toolkit contains ported **custom controls** and some features that are not restricted **UWP only** from **WCT**.

Parts of the WCT which target **.NET Standard** (as you can use them from any .NET library/application) are not ported. However, some WinUI (UWP XAML) specific features are ported.

This toolkit also contains some additional controls and helpers that are **WPF or ModernWpf specific**.

So, **ModernWpf** ≋ **WinUI** and **MCT** ≋ **WCT**.

## NuGet Packages
| NuGet Package | Latest Versions |
| --- | --- |
| [ModernWpf.Toolkit][Toolkit] | [![latest stable version](https://img.shields.io/nuget/v/ModernWpf.Toolkit)][Toolkit]<br />[![latest prerelease version](https://img.shields.io/nuget/vpre/ModernWpf.Toolkit)][Toolkit.Pre] |
| [ModernWpf.Toolkit.UI][Toolkit.UI] | [![latest stable version](https://img.shields.io/nuget/v/ModernWpf.Toolkit.UI)][Toolkit.UI]<br />[![latest prerelease version](https://img.shields.io/nuget/vpre/ModernWpf.Toolkit.UI)][Toolkit.UI.Pre] |
| [ModernWpf.Toolkit.UI.Controls][Toolkit.UI.Controls] | [![latest stable version](https://img.shields.io/nuget/v/ModernWpf.Toolkit.UI.Controls)][Toolkit.UI.Controls]<br />[![latest prerelease version](https://img.shields.io/nuget/vpre/ModernWpf.Toolkit.UI.Controls)][Toolkit.UI.Controls.Pre] |
| [ModernWpf.Toolkit.UI.Controls.Markdown][Toolkit.UI.Controls.Markdown] | [![latest stable version](https://img.shields.io/nuget/v/ModernWpf.Toolkit.UI.Controls.Markdown)][Toolkit.UI.Controls.Markdown]<br />[![latest prerelease version](https://img.shields.io/nuget/vpre/ModernWpf.Toolkit.UI.Controls.Markdown)][Toolkit.UI.Controls.Markdown.Pre] |

[Toolkit]: https://www.nuget.org/packages/ModernWpf.Toolkit/
[Toolkit.Pre]: https://www.nuget.org/packages/ModernWpf.Toolkit/absoluteLatest
[Toolkit.UI]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI/
[Toolkit.UI.Pre]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI/absoluteLatest
[Toolkit.UI.Controls]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI.Controls/
[Toolkit.UI.Controls.Pre]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI.Controls/absoluteLatest
[Toolkit.UI.Controls.Markdown]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI.Controls.Markdown/
[Toolkit.UI.Controls.Markdown.Pre]: https://www.nuget.org/packages/ModernWpf.Toolkit.UI.Controls.Markdown/absoluteLatest
