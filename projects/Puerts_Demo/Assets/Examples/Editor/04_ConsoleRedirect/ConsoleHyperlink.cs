using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.CodeEditor;

namespace UnityEditor.Console
{
    public static class ConsoleHyperlink
    {
        static EventInfo _hyperLinkClickedEvent;
        static EventInfo hyperLinkClickedEvent
        {
            get
            {
                if (_hyperLinkClickedEvent == null)
                {
                    _hyperLinkClickedEvent = typeof(EditorGUI).GetEvent("hyperLinkClicked", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                }
                return _hyperLinkClickedEvent;
            }
        }

        static Delegate _hyperLinkClickedHandler;
        static Delegate hyperLinkClickedHandler
        {
            get
            {
                if (_hyperLinkClickedHandler == null)
                {
#if UNITY_2021_2_OR_NEWER
                    Action<EditorWindow, HyperLinkClickedEventArgs> handler = (_, args) => OnProcessClickData(args.hyperLinkData);
                    _hyperLinkClickedHandler = handler;
#else
                    if (hyperLinkClickedEvent != null)
                    {
                        var method = typeof(UnityEditor.Console.ConsoleHyperlink).GetMethod("OnClicked", BindingFlags.Static | BindingFlags.NonPublic| BindingFlags.Public);
                        if (method != null)
                        {
                            _hyperLinkClickedHandler = Delegate.CreateDelegate(hyperLinkClickedEvent.EventHandlerType, method);
                        }
                    }
#endif
                }
                return _hyperLinkClickedHandler;
            }
        }

        public static void Enable()
        {
            if (hyperLinkClickedHandler == null)
                throw new NotSupportedException();

#if UNITY_2021_2_OR_NEWER
            EditorGUI.hyperLinkClicked += (Action<EditorWindow, HyperLinkClickedEventArgs>)hyperLinkClickedHandler;
#else
            hyperLinkClickedEvent.AddMethod.Invoke(null, new object[] { hyperLinkClickedHandler });
#endif
        }
        public static void Disable()
        {
            if (hyperLinkClickedHandler == null)
                throw new NotSupportedException();

#if UNITY_2021_2_OR_NEWER
            EditorGUI.hyperLinkClicked -= (Action<EditorWindow, HyperLinkClickedEventArgs>)hyperLinkClickedHandler;
#else
            hyperLinkClickedEvent.RemoveMethod.Invoke(null, new object[] { hyperLinkClickedHandler });
#endif  
        }

        static PropertyInfo property;
        static void OnClicked(object sender, EventArgs args)
        {
            if (property == null)
            {
                property = args.GetType().GetProperty("hyperlinkInfos", BindingFlags.Instance | BindingFlags.Public);
                if (property == null) return;
            }
            var infos = property.GetValue(args);
            if (infos is Dictionary<string, string>)
            {
                OnProcessClickData((Dictionary<string, string>)infos);
            }
        }

        static void OnProcessClickData(Dictionary<string, string> infos)
        {
            if (infos == null) return;
            if (!infos.TryGetValue("href", out var path)) return;
            infos.TryGetValue("line", out var line);
            infos.TryGetValue("column", out var column);

            if (File.Exists(path))
            {
                int.TryParse(line, out var _line);
                int.TryParse(column, out var _column);
                OpenFileInIDE(path, _line, _column);
            }
        }

        public static bool OpenFileInIDE(string filepath, int line, int column)
        {
            return CodeEditor.CurrentEditor.OpenProject(filepath, line, column);
        }
    }
}