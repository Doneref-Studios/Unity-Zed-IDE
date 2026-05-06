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

            // always add project path
            var args = new StringBuilder($"\"{m_ProjectPath.ToString(SlashMode.Native)}\"");

            // if file path is provided, add it as a second positional argument
            if (!string.IsNullOrEmpty(filePath))
            {
                args.Append($" \"{filePath}");

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
