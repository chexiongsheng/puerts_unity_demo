using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// 将注入的方法作为单独的程序集包装(避免注入后程序集冲突, 从而影响Assembly-CSharp-Editor.dll程序集正常加载)
/// </summary>
namespace UnityEditor.Console
{
    using UnityEditor.Console.Callbacks;
    using UnityEngine;

    public static class OnClicked
    {
        static BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        static bool OnLinkClicked(object[] parameeters)
        {
            if (parameeters != null && parameeters.Length > 0 && parameeters[parameeters.Length - 1] is System.EventArgs eventArg)
            {
                //Debug.Log($"Method: { parameeters[0] } , Parameters: { string.Join(",", parameeters.Skip(1)) }");
                PropertyInfo propInfo = eventArg.GetType().GetProperty("hyperlinkInfos", instanceFlags);

                Dictionary<string, string> hyperlinkInfos = propInfo != null ? propInfo.GetValue(eventArg) as Dictionary<string, string> : null;
                if (hyperlinkInfos != null && hyperlinkInfos.TryGetValue("href", out var href))
                {
                    string line, column;
                    hyperlinkInfos.TryGetValue("line", out line);
                    hyperlinkInfos.TryGetValue("column", out column);

                    return Callback(href, !string.IsNullOrEmpty(line) ? int.Parse(line) : 0, !string.IsNullOrEmpty(column) ? int.Parse(column) : 0);
                }
            }
            return false;
        }

        static void OnTest(object[] parameeters)
        {
            Debug.Log($"Method: { parameeters[0] } , Parameters: { string.Join(", ", parameeters.Skip(1)) }");
        }

        static Subscriber[] subscribers;
        static bool Callback(string href, int line, int column)
        {
            if (subscribers == null)
            {
                BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                subscribers = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetExportedTypes()
                               where type.IsAbstract && type.IsSealed
                               from method in type.GetMethods(flags)
                               where method.IsDefined(typeof(OnOpenAttribute))
                               select new Subscriber(method)
                            )
                            .Where(o => o.ready)
                            .ToArray();
                Array.Sort(subscribers, (s1, s2) => s1.order < s2.order ? -1 : s1.order > s2.order ? 1 : 0);
            }
            foreach (Subscriber subscriber in subscribers)
            {
                object result = subscriber.Invoke(href, line, column);
                if (result is bool && (bool)result)
                {
                    return true;
                }
            }
            return false;
        }
    }
    class Subscriber
    {
        public int order { get; private set; }
        public bool ready { get; private set; }
        private int paramLen;
        private MethodInfo method;
        public Subscriber(MethodInfo method)
        {
            Type[] parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
            this.method = method;
            this.paramLen = parameters.Length;
            this.ready = parameters.Length >= 1 && parameters.Length <= 3 && parameters[0] == typeof(string) &&
                (parameters.Length == 1 || parameters[1] == typeof(int)) &&
                (parameters.Length == 2 || parameters[2] == typeof(int));
            this.order = method.GetCustomAttributes<OnOpenAttribute>().FirstOrDefault()?.order ?? 0;
        }

        public object Invoke(string href, int line, int column)
        {
            if (!this.ready)
            {
                return default;
            }
            switch (this.paramLen)
            {
                case 1:
                    return this.method.Invoke(null, new object[] { href });
                case 2:
                    return this.method.Invoke(null, new object[] { href, line });
                default:
                    return this.method.Invoke(null, new object[] { href, line, column });

            }
        }
    }
}
namespace UnityEditor.Console.Callbacks
{
    [AttributeUsageAttribute(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class OnOpenAttribute : Attribute
    {
        public int order { get; private set; }
        public OnOpenAttribute() : this(0) { }
        public OnOpenAttribute(int order)
        {
            this.order = order;
        }
    }
}
