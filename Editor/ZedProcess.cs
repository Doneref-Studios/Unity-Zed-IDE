using System.Text;
using NiceIO;
using Unity.CodeEditor;
using UnityEngine;

namespace UnityZed
{
    public class ZedProcess
    {
        private static readonly ILogger sLogger = ZedLogger.Create();

        private readonly NPath m_ExecPath;
        private readonly NPath m_ProjectPath;

        public ZedProcess(string execPath)
        {
            m_ExecPath = execPath;
            m_ProjectPath = new NPath(Application.dataPath).Parent;
        }

        public bool OpenProject(string filePath = "", int line = -1, int column = -1)
        {
            sLogger.Log("OpenProject");

            // Always pass the project folder first so Zed uses it as the workspace root.
            // Normalize both paths to native slashes — mixing styles (e.g. C:\project vs
            // C:/Assets/Script.cs) causes Zed on Windows to treat them as unrelated paths
            // and open two separate workspace roots instead of one.
            var args = new StringBuilder($"\"{m_ProjectPath.ToString(SlashMode.Native)}\"");

            if (!string.IsNullOrEmpty(filePath))
            {
                var nativePath = new NPath(filePath).ToString(SlashMode.Native);
                args.Append($" \"{nativePath}");

                if (line >= 0)
                {
                    args.Append(":");
                    args.Append(line);

                    if (column >= 0)
                    {
                        args.Append(":");
                        args.Append(column);
                    }
                }
                args.Append("\"");
            }

#if UNITY_EDITOR_WIN
            // On Windows, CodeEditor.OSOpenFile shows a console window for CLI executables.
            // Use Process.Start directly with CreateNoWindow to suppress it.
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = m_ExecPath.ToString(SlashMode.Native),
                Arguments = args.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            System.Diagnostics.Process.Start(startInfo);
            return true;
#else
            return CodeEditor.OSOpenFile(m_ExecPath.ToString(SlashMode.Native), args.ToString());
#endif
        }
    }
}
