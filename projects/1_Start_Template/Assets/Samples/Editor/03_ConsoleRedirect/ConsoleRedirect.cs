using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.CodeEditor;
using UnityEngine;

namespace UnityEditor.Console
{
    public static class ConsoleRedirect
    {
        //匹配文件路径(指定.js|.cjs|.mjs|.ts|.mts后缀)^\n\r\*\"\|\<\>
        static readonly Regex regex = new Regex("at ([a-zA-z0-9#$._ ]+ \\()?([^\n\r*\"|<>]+(.js|.cjs|.mjs|.ts|.mts))\\:([0-9]+)\\:([0-9]+)\\)?\r?\n?");
        //匹配<a href ...>(指定.js|.cjs|.mjs|.ts|.mts后缀)
        static readonly Regex regexHref = new Regex("<a href='([^\n\r*\"|<>]+(.js|.cjs|.mjs|.ts|.mts))'( line='([0-9]+)')?( column='([0-9]+)')?>[^\n\r*\"|<>]+</a>"
            .Replace('\'', '\"'));

        public static bool enable
        {
            get { return EditorPrefs.GetBool("Editor.ConsoleRedirect.Enable", true); }
            private set { EditorPrefs.SetBool("Editor.ConsoleRedirect.Enable", value); }
        }
        [MenuItem("Tools/ConsoleRedirect/Enable")]
        static void Enable()
        {
            enable = true;
            UnityEditor.Console.ConsoleHyperlink.Enable();
        }
        [MenuItem("Tools/ConsoleRedirect/Enable", true)]
        static bool EnableCheck() { return !enable; }
        [MenuItem("Tools/ConsoleRedirect/Disable")]
        static void Disable()
        {
            enable = false;
            UnityEditor.Console.ConsoleHyperlink.Disable();
        }
        [MenuItem("Tools/ConsoleRedirect/Disable", true)]
        static bool DisableCheck() { return enable; }


        [InitializeOnLoadMethod]
        static void Init()
        {
            if (!enable)
                return;

            UnityEditor.Console.ConsoleHyperlink.Enable();
        }

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        static bool OnOpen(int instanceID, int lineId)
        {
            if (!enable)
                return false;

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


        static bool OpenFileInIDE(string filepath, int line, int column)
        {
            return CodeEditor.CurrentEditor.OpenProject(filepath, line, column);
        }

        public static string GetStackTrace()
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
