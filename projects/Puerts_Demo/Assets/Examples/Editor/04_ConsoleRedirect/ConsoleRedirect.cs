using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.Console
{
    public static class ConsoleRedirect
    {
        const string IDE_PATH = "IDE_PATH_KEY";
        const string PROJECT_PATH = "PROJECT_PATH_KEY";

        //匹配文件路径(指定js/ts后缀)
        static readonly Regex regex = new Regex(@"at ([a-zA-z0-9#$._ ]+ \()?([a-zA-Z0-9:/\\._ ]+(.js|.ts))\:([0-9]+)\:([0-9]+)\)?\r?\n?");
        //匹配<a href ...>(指定js/ts后缀)
        static readonly Regex regexHref = new Regex(@"<a href='([a-zA-Z0-9:/\\. ]+(.js|.ts))'( line='([0-9]+)')?( column='([0-9]+)')?>[a-zA-Z0-9:/\\. ]+</a>"
            .Replace('\'', '\"'));

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        static bool OnOpen(int instanceID, int lineId)
        {
            string track = GetStackTrace();
            if (string.IsNullOrEmpty(track))
                return false;

            //匹配<a href= ...>
            Match match = regexHref.Match(track);
            while (match.Success)
            {
                try
                {
                    string filepath = match.Groups[1].Value;
                    string line = match.Groups[4].Value;
                    string column = match.Groups[6].Value;
                    if (!string.IsNullOrEmpty(line) && File.Exists(filepath))
                    {
                        return OpenFileInIDE(filepath, !string.IsNullOrEmpty(line) ? int.Parse(line) : 0, !string.IsNullOrEmpty(column) ? int.Parse(column) : 0);
                    }
                }
                catch (Exception) { return false; }

                match = match.NextMatch();
            }
            //匹配文件路径/后缀名/行数/列数
            match = regex.Match(track);
            while (match.Success)
            {
                try
                {
                    string filepath = match.Groups[2].Value;
                    string line = match.Groups[4].Value;
                    string column = match.Groups[5].Value;
                    if (File.Exists(filepath))
                    {
                        return OpenFileInIDE(filepath, !string.IsNullOrEmpty(line) ? int.Parse(line) : 0, !string.IsNullOrEmpty(column) ? int.Parse(column) : 0);
                    }
                }
                catch (Exception) { return false; }
                match = match.NextMatch();
            }

            return false;
        }
        //如果href指向的文件不在Assets或Packages中, UnityEditor.Callbacks.OnOpenAsset将不会传参instanceID和lineId
        //因此需要执行InjectTool.Inject注入UnityEditor.ConsoleWindow.EditorGUI_HyperLinkClicked函数, 拿到eventArg参数自定义打开文件(替换UnityEditor.dll)
        [UnityEditor.Console.Callbacks.OnOpen(0)]
        static bool OnOpen_Custom(string href, int line)
        {
            if (File.Exists(href))
            {
                return OpenFileInIDE(href, line, 0);
            }
            href = Path.Combine(Application.dataPath, href);
            if (File.Exists(href))
            {
                return OpenFileInIDE(href, line, 0);
            }

            return false;
        }

        [MenuItem("Tools/Console/DLL/Inject")]
        public static void Inject()
        {
            InjectTool.Inject();
        }
        [MenuItem("Tools/Console/DLL/Restore")]
        public static void RestoreOriginal()
        {
            InjectTool.RestoreOriginal();
        }
        [MenuItem("Tools/Console/DLL/Folder")]
        public static void OpenDLLFolder()
        {
            InjectTool.OpenDLLFolder();
        }


        [MenuItem("Tools/Console/Set IDE Path")]
        static void SetEditorPath()
        {
            string path = EditorUserSettings.GetConfigValue(IDE_PATH);
            path = EditorUtility.OpenFilePanel("Select Your IDE", path, "exe");
            if (!string.IsNullOrEmpty(path))
            {
                EditorUserSettings.SetConfigValue(IDE_PATH, path);
                Debug.Log("IDE Path: " + path);
            }
        }
        [MenuItem("Tools/Console/Set Project Path")]
        static void SetProjectPath()
        {
            string path = EditorUserSettings.GetConfigValue(PROJECT_PATH);
            path = EditorUtility.OpenFolderPanel("Select Your Project", path, "");
            if (!string.IsNullOrEmpty(path))
            {
                EditorUserSettings.SetConfigValue(PROJECT_PATH, path);
                Debug.Log("Project Path: " + path);
            }
        }
        [MenuItem("Tools/Console/PrintPaths")]
        static void PrintPaths()
        {
            Debug.Log("IDE Path: " + EditorUserSettings.GetConfigValue(IDE_PATH));
            Debug.Log("Project Path: " + EditorUserSettings.GetConfigValue(PROJECT_PATH));
        }

        static bool OpenFileInIDE(string filepath, int line, int column)
        {
            string editorPath = EditorUserSettings.GetConfigValue(IDE_PATH);
            if (string.IsNullOrEmpty(editorPath) || !File.Exists(editorPath))
            {
                SetEditorPath();
                editorPath = EditorUserSettings.GetConfigValue(IDE_PATH);
                if (string.IsNullOrEmpty(editorPath))
                    return false;
            }
            string projectPath = EditorUserSettings.GetConfigValue(PROJECT_PATH);
            if (string.IsNullOrEmpty(projectPath) || !Directory.Exists(projectPath))
            {
                SetProjectPath();
                projectPath = EditorUserSettings.GetConfigValue(PROJECT_PATH);
                if (string.IsNullOrEmpty(projectPath))
                    return false;
            }
            string args = null;
            if (editorPath.EndsWith("Code.exe"))   //VScode的启动参数
            {
                args = string.Format("\"{0}\" -g \"{1}\":{2}:{3}", projectPath, filepath, line, column);
            }
            else
            {
                Debug.LogError("Unknown IDE config");
                return false;
            }
            //默认为VScode的启动参数

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = editorPath;
            proc.StartInfo.Arguments = args;
            proc.Start();
            return true;
        }
        static string GetStackTrace()
        {
            var editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var consoleWindowType = editorAssembly.GetType("UnityEditor.ConsoleWindow", false);
            var fieldInfo = consoleWindowType.GetField(
                                    "ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);

            var consoleWindow = fieldInfo.GetValue(null) as EditorWindow;
            if (consoleWindow != EditorWindow.focusedWindow)
            {
                return null;
            }
            var activeTextFieldInfo = consoleWindowType.GetField(
                                       "m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);

            return (string)activeTextFieldInfo.GetValue(consoleWindow);
        }
    }
}
