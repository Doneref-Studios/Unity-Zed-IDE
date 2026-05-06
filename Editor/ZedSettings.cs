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

        public static bool InjectSolutionPath
        {
            get => UnityEditor.EditorPrefs.GetBool(kInjectSolutionPathKey, true);
            set => UnityEditor.EditorPrefs.SetBool(kInjectSolutionPathKey, value);
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
