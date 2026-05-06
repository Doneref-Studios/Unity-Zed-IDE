# Unity Zed IDE

![Platforms](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-blue)
![Unity](https://img.shields.io/badge/Unity-2022.3%2B-orange)
![Version](https://img.shields.io/badge/version-1.0.0-green)

A Unity package by **[Doneref Studios](https://github.com/Doneref-Studios)** that integrates the [Zed](https://zed.dev) code editor as Unity's external script editor, with full support for C# project generation and file-open workflows across **Windows**, **macOS**, and **Linux**.

## Features

- **Auto-discovery** - Automatically detects Zed installations on all supported platforms
- **C# project generation** - Generates `.sln` and `.csproj` files via the Visual Studio package backend
- **File navigation** - Opens files at the correct line and column directly from Unity's console or by double-clicking assets
- **Preferences UI** - Configure project generation options from Unity's Preferences window

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

### 1. Install the package

Open **Window > Package Management > Package Manager**, click the **+** button in the top-left corner, and select **Install package from git URL...**

![Package Manager install from git URL](https://raw.githubusercontent.com/Doneref-Studios/Unity-Zed-IDE/main/.github/images/install.png)

Paste the following URL and click **Install**:

```
https://github.com/Doneref-Studios/Unity-Zed-IDE.git
```

Wait for Unity to download and compile the package.

### 2. Install the Zed C# extension

Open Zed, press `Ctrl+Shift+X` to open the extension panel, search for **C#**, and install it.
Alternatively, visit the extension page: [zed-extensions/csharp](https://github.com/zed-extensions/csharp)

The C# extension uses the **Roslyn** language server, which requires the **.NET 10 runtime**.
Download and install it from:

```
https://dotnet.microsoft.com/download/dotnet/10.0
```

Without .NET 10, features like go-to-definition (F12), auto-complete, and inline diagnostics will not work.

### 3. Set Zed as your script editor

Open **Edit > Preferences > External Tools** and set **External Script Editor** to **Zed**.

![External Tools preferences showing Zed selected](https://raw.githubusercontent.com/Doneref-Studios/Unity-Zed-IDE/main/.github/images/preferences.png)

Configure which project types should have `.csproj` files generated, then click **Regenerate project files**.

The **Inject solution path (recommended)** toggle controls whether the package automatically writes the solution file path into `.zed/settings.json`. This tells the Roslyn language server to load only your project's solution instead of scanning all packages, which prevents go-to-definition (F12) timeouts. Leave it enabled unless you manage `.zed/settings.json` manually.

## Troubleshooting

### Go-to-definition (F12) stops working or Zed becomes unresponsive

This happens when Roslyn has no `.sln` file to anchor to and scans hundreds of package `.csproj` files instead, causing 120s request timeouts.

**Fix:**
1. In Unity, open **Edit > Preferences > External Tools**
2. Make sure **Inject solution path (recommended)** is enabled
3. Click **Regenerate project files**

This creates the `.sln` and updates `.zed/settings.json` with:

```json
{
  "lsp": {
    "roslyn": {
      "initialization_options": {
        "solutionPath": "YourProject.sln"
      }
    }
  }
}
```

Restart Zed after regenerating. The first load takes ~30s while Roslyn indexes the solution.

## Acknowledgements

Forked from [Maligan/unity-zed](https://github.com/Maligan/unity-zed).

Windows process and path fixes contributed by the upstream community (see [PR #16](https://github.com/Maligan/unity-zed/pull/16)).

## License

[MIT](LICENSE.txt)

