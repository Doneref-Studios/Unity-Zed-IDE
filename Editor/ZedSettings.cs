using UnityEngine;
using NiceIO;
using SimpleJSON;
using System.IO;

namespace UnityZed
{
    public class ZedSettings
    {
        private static readonly ILogger sLogger = ZedLogger.Create();

        private const string kInjectSolutionPathKey = "UnityZed.InjectSolutionPath";
        private const string kInjectUnityMcpKey = "UnityZed.InjectUnityMcp";

        public static bool InjectSolutionPath
        {
            get => UnityEditor.EditorPrefs.GetBool(kInjectSolutionPathKey, true);
            set => UnityEditor.EditorPrefs.SetBool(kInjectSolutionPathKey, value);
        }

        public static bool InjectUnityMcp
        {
            get => UnityEditor.EditorPrefs.GetBool(kInjectUnityMcpKey, false);
            set => UnityEditor.EditorPrefs.SetBool(kInjectUnityMcpKey, value);
        }

        private static NPath GetRelayPath()
        {
            var home = new NPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));
#if UNITY_EDITOR_WIN
            return home.Combine(".unity/relay/relay_win.exe");
#elif UNITY_EDITOR_OSX
            var arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
            if (arch == System.Runtime.InteropServices.Architecture.Arm64)
                return home.Combine(".unity/relay/relay_mac_arm64.app/Contents/MacOS/relay_mac_arm64");
            else
                return home.Combine(".unity/relay/relay_mac_x64.app/Contents/MacOS/relay_mac_x64");
#else
            return home.Combine(".unity/relay/relay_linux");
#endif
        }

        // Global Zed settings file — this is where context_servers must live for the AI agent to pick them up.
        // Project-level .zed/settings.json is only for editor config (LSP, themes, etc.).
        private static NPath GetGlobalZedSettingsPath()
        {
#if UNITY_EDITOR_WIN
            return new NPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData))
                .Combine("Zed/settings.json");
#else
            return new NPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile))
                .Combine(".config/zed/settings.json");
#endif
        }

        private readonly NPath m_SettingsPath;
        private readonly NPath m_ProjectRoot;

        public ZedSettings()
        {
            m_ProjectRoot = new NPath(UnityEngine.Application.dataPath).Parent;
            m_SettingsPath = m_ProjectRoot.Combine(".zed/settings.json");
        }

        public void Sync()
        {
            if (m_SettingsPath.FileExists() == false)
            {
                sLogger.Log("Zed settings file not found, creating default settings file.");
                m_SettingsPath.CreateFile();
                m_SettingsPath.WriteAllText(JSON.Parse(kDefaultSettings).ToString());
            }

            if (InjectSolutionPath)
                SyncSolutionPath();

            SyncUnityMcp();
        }

        private void SyncUnityMcp()
        {
            var globalSettingsPath = GetGlobalZedSettingsPath();
            if (!globalSettingsPath.FileExists())
            {
                sLogger.Log($"Global Zed settings not found at '{globalSettingsPath}'. Is Zed installed?");
                return;
            }

            var json = JSON.Parse(globalSettingsPath.ReadAllText()) ?? new JSONObject();

            if (InjectUnityMcp)
            {
                var relayPath = GetRelayPath();
                if (!relayPath.FileExists())
                {
                    sLogger.Log($"Unity MCP relay not found at '{relayPath}'. Is the Unity AI Assistant package installed?");
                    return;
                }

                var relayPathStr = relayPath.ToString(SlashMode.Native);

                if (json["context_servers"] == null) json["context_servers"] = new JSONObject();
                if (json["context_servers"]["unity-mcp"] == null) json["context_servers"]["unity-mcp"] = new JSONObject();

                var entry = json["context_servers"]["unity-mcp"];
                var currentCommand = entry["command"]?.Value;
                var currentArg = entry["args"]?.Count == 1 ? entry["args"][0]?.Value : null;

                if (currentCommand == relayPathStr && currentArg == "--mcp")
                    return;

                entry["command"] = relayPathStr;
                var args = new JSONArray();
                args.Add("--mcp");
                entry["args"] = args;

                globalSettingsPath.WriteAllText(json.ToString());
                sLogger.Log($"Zed settings: Unity MCP server configured in global settings (relay: '{relayPathStr}').");
            }
            else
            {
                if (json["context_servers"] == null || json["context_servers"]["unity-mcp"] == null)
                    return;

                json["context_servers"].Remove("unity-mcp");
                globalSettingsPath.WriteAllText(json.ToString());
                sLogger.Log("Zed settings: Unity MCP server entry removed from global settings.");
            }
        }

        private void SyncSolutionPath()
        {
            var slnFiles = m_ProjectRoot.Files("*.sln");
            if (slnFiles.Length == 0)
                return;

            var slnName = slnFiles[0].FileName;

            var json = JSON.Parse(m_SettingsPath.ReadAllText()) ?? new JSONObject();

            if (json["lsp"] == null) json["lsp"] = new JSONObject();
            if (json["lsp"]["roslyn"] == null) json["lsp"]["roslyn"] = new JSONObject();
            if (json["lsp"]["roslyn"]["initialization_options"] == null)
                json["lsp"]["roslyn"]["initialization_options"] = new JSONObject();

            var current = json["lsp"]["roslyn"]["initialization_options"]["solutionPath"]?.Value;
            if (current == slnName)
                return;

            json["lsp"]["roslyn"]["initialization_options"]["solutionPath"] = slnName;
            m_SettingsPath.WriteAllText(json.ToString());
            sLogger.Log($"Zed settings: solutionPath set to '{slnName}'.");
        }

        private const string kDefaultSettings = @"{
            ""file_scan_exclusions"": [
                ""**/.*"",
                ""**/*~"",

                ""*.csproj"",
                ""*.sln"",

                ""**/*.meta"",
                ""**/*.booproj"",
                ""**/*.pibd"",
                ""**/*.suo"",
                ""**/*.user"",
                ""**/*.userprefs"",
                ""**/*.unityproj"",
                ""**/*.dll"",
                ""**/*.exe"",
                ""**/*.pdf"",
                ""**/*.mid"",
                ""**/*.midi"",
                ""**/*.wav"",
                ""**/*.gif"",
                ""**/*.ico"",
                ""**/*.jpg"",
                ""**/*.jpeg"",
                ""**/*.png"",
                ""**/*.psd"",
                ""**/*.tga"",
                ""**/*.tif"",
                ""**/*.tiff"",
                ""**/*.3ds"",
                ""**/*.3DS"",
                ""**/*.fbx"",
                ""**/*.FBX"",
                ""**/*.lxo"",
                ""**/*.LXO"",
                ""**/*.ma"",
                ""**/*.MA"",
                ""**/*.obj"",
                ""**/*.OBJ"",
                ""**/*.asset"",
                ""**/*.cubemap"",
                ""**/*.flare"",
                ""**/*.mat"",
                ""**/*.meta"",
                ""**/*.prefab"",
                ""**/*.unity"",

                ""build/"",
                ""Build/"",
                ""library/"",
                ""Library/"",
                ""obj/"",
                ""Obj/"",
                ""Packages/"",
                ""ProjectSettings/"",
                ""UserSettings/"",
                ""temp/"",
                ""Temp/"",
                ""logs"",
                ""Logs"",
            ]
        }";
    }
}
