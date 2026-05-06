# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.0] - 2026-05-06

### Added
- Windows support: auto-discovery of Zed via `%LOCALAPPDATA%/Programs/Zed`
- Suppress terminal popup on Windows using `ProcessStartInfo` with `CreateNoWindow = true`
- Author, license, changelog, and documentation URLs in `package.json`
- Screenshots for Package Manager and Preferences setup in README
- C# extension and .NET 10 runtime setup instructions in README

### Fixed
- `NullReferenceException` in `ZedPreferences.OnGUI` when running from `Assets/` (null guard on `PackageInfo.FindForAssembly`)
- Invalid `/a` flag being passed to Zed CLI, which created a spurious empty file named `a`
- Forward-slash paths on Windows (`SlashMode.Native` applied to all paths)

### Changed
- Package renamed from `com.maligan.unity-zed` to `com.doneref.unity-zed-ide`
- README rewritten with GUI-based install instructions
- Roadmap section removed

---

## Upstream history (Maligan/unity-zed)

### [0.2.3-preview]
- Bump version

### [0.2.2-preview]
- Add Homebrew installs of Zed in discovery

### [0.2.1-preview]
- Add Zed location for NixOS
- Additional Linux path fixes

### [0.2.0-preview]
- Add `.zed/settings.json` generator

### [0.1.1-preview]
- Fix empty `filePath` opening

### [0.1.0-alpha]
- Add `.sln`/`.csproj` generator support

### [0.0.4-alpha]
- Fix Open C# Project with empty `filePath`

### [0.0.3-alpha]
- Initial Linux Flatpak global path support
- Fix spaces in project/file path

### [0.0.2-alpha] / [0.0.1-alpha]
- Initial release
