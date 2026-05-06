# Unity Zed IDE

[![openupm](https://img.shields.io/npm/v/com.maligan.unity-zed?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.maligan.unity-zed/)
![Platforms](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-blue)
![Unity](https://img.shields.io/badge/Unity-2022.3%2B-orange)

Integrates the [Zed](https://zed.dev) code editor as the external script editor in Unity, with full support for C# project generation and file-open workflows across **Windows**, **macOS**, and **Linux**.

## Features

- **Auto-discovery** — Automatically detects Zed installations on all supported platforms
- **C# project generation** — Generates `.sln` and `.csproj` files via the Visual Studio package backend
- **File navigation** — Opens files at the correct line and column directly from Unity's console or double-clicking assets
- **Preferences UI** — Configure project generation options from Unity's Preferences window

## Platform Support

| Platform | Status |
|----------|--------|
| Windows  | ✅ Supported |
| macOS    | ✅ Supported |
| Linux    | ✅ Supported (Official, Flatpak, NixOS, Repo) |

## Requirements

- Unity **2022.3** or later
- [Zed](https://zed.dev) installed on your system
- `com.unity.ide.visualstudio` package (resolved automatically as a dependency)

## Installation

**Via Unity Package Manager (Git URL)**

In your project's `Packages/manifest.json`, add:

```json
"com.doneref.unity-zed-ide": "https://github.com/Doneref-Studios/Unity-Zed-IDE.git"
```

Or open the Package Manager window, choose **Add package from git URL…**, and paste the URL above.

**Directly in Assets**

Copy the `Editor/` folder into `Assets/Plugins/unity-zed/`. The `.asmdef` will ensure the scripts are compiled only for the Editor.

## Setup

1. Open **Edit → Preferences → External Tools**
2. Set **External Script Editor** to **Zed**
3. Configure which project types should have `.csproj` files generated under the Zed preferences section

## Roadmap

- [x] Zed installation discovery (Windows, macOS, Linux)
- [x] Register as Unity external script editor
- [x] C# `.sln` / `.csproj` generation
- [x] Correct file / line / column open workflow on all platforms
- [x] Graceful fallback when not running as a Unity package
- [ ] Zed extension for deeper Unity integration via LSP/IPC

## Acknowledgements

Forked from [Maligan/unity-zed](https://github.com/Maligan/unity-zed). Windows process and path fixes contributed by the upstream community (see [PR #16](https://github.com/Maligan/unity-zed/pull/16)).

## License

[MIT](LICENSE.txt)

