using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Puerts.Editor
{
    public static class GeneratorUsing
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public class GenInfo
        {
            public string Name;
            public string[] Parameters;
            public bool HasReturn;

            public override bool Equals(object obj)
            {
                if (obj != null && obj is GenInfo)
                {
                    var info = (GenInfo)obj;
                    if (this.Name != info.Name || this.Parameters.Length != info.Parameters.Length || this.HasReturn != info.HasReturn)
                        return false;
                    for (int i = 0; i < this.Parameters.Length; i++)
                    {
                        if (this.Parameters[i] != info.Parameters[i])
                            return false;
                    }
                    return true;
                }
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return string.Format("{0}<{1}>", Name, string.Join(", ", Parameters));
            }
        }
        static string GetFullname(Type type)
        {
            if (type.IsGenericType)
            {
                var fullName = string.IsNullOrEmpty(type.FullName) ? type.ToString() : type.FullName;
                var parts = fullName.Replace('+', '.').Split('`');
                var argTypenames = type.GetGenericArguments()
                    .Select(x => GetFullname(x)).ToArray();
                return parts[0] + "<" + string.Join(", ", argTypenames) + ">";
            }
            if (!string.IsNullOrEmpty(type.FullName))
                return type.FullName.Replace('+', '.');
            return type.ToString();
        }
        static GenInfo ToGenInfo(Type type)
        {
            var invoke = type.GetMethod("Invoke", Flags);
            var parameters = (from p in invoke.GetParameters() select GetFullname(p.ParameterType)).ToList();
            var hasReturn = invoke.ReturnType != typeof(void);
            if (hasReturn)
                parameters.Add(GetFullname(invoke.ReturnType));
            return new GenInfo()
            {
                Name = hasReturn ? "UsingFunc" : "UsingAction",
                Parameters = parameters.ToArray(),
                HasReturn = hasReturn
            };
        }
        static bool IsDelegate(Type type)
        {
            if (type == null)
            {
                return false;
            }
            else if (type == typeof(Delegate))
            {
                return true;
            }
            else
            {
                return IsDelegate(type.BaseType);
            }
        }
        static void AddRef(List<Type> refs, Type type)
        {
            if (type.IsArray)
            {
                AddRef(refs, type.GetElementType());
            }
            else if (IsDelegate(type) && !type.IsGenericParameter && type != typeof(Delegate) && type != typeof(MulticastDelegate))
            {
                refs.Add(type);
            }
        }
        static bool IsSafeType(Type type)
        {
            var fullname = type.FullName;
            if (fullname != null && (fullname.Contains("*") || fullname.Contains("&")))
            {
                return false;
            }
            return true;
        }
        static bool IsGenericType(Type type)
        {
            if (type.IsGenericParameter)
                return true;
            if (type.IsGenericType)
            {
                return type.GetGenericArguments().FirstOrDefault(argType => IsGenericType(argType)) != null;
            }
            return false;
        }
        static bool ContainsValueParameter(Type type)
        {
            var invoke = type.GetMethod("Invoke", Flags);
            if (invoke != null)
            {
                var containsValueType = invoke.ReturnType != typeof(void) && invoke.ReturnType.IsValueType;
                foreach (var param in invoke.GetParameters())
                {
                    if (IsGenericType(param.ParameterType) || !IsSafeType(param.ParameterType))
                        return false;
                    if (param.ParameterType.IsValueType)
                        containsValueType = true;
                }
                return containsValueType && !IsGenericType(invoke.ReturnType) && IsSafeType(invoke.ReturnType);
            }
            return false;
        }
        static List<int> AllowArgumentsLength(string methodName)
        {
            return typeof(Puerts.JsEnv).GetMethods(Flags)
                .Where(m => m.IsGenericMethod && m.Name == methodName)
                .Select(m => m.GetGenericArguments().Length)
                .ToList();
        }

        [MenuItem("PuertsEditorDemo/Generate UsingCode", false, 1)]
        public static void GenerateUsingCode()
        {
            var start = DateTime.Now;
            var saveTo = Configure.GetCodeOutputDirectory();
            Directory.CreateDirectory(saveTo);
            GenerateCode(saveTo);
            Debug.Log("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
            AssetDatabase.Refresh();
        }

        static void GenerateCode(string saveTo)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                "Puerts.BindingAttribute",
            });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => !t.IsGenericTypeDefinition);

            var refTypes = new List<Type>();
            foreach (var type in genTypes)
            {
                AddRef(refTypes, type);
                foreach (var field in type.GetFields(Flags))
                {
                    AddRef(refTypes, field.FieldType);
                }
                foreach (var prop in type.GetProperties(Flags))
                {
                    AddRef(refTypes, (prop.CanRead ? prop.GetMethod : prop.SetMethod).ReturnType);
                }
                foreach (var method in type.GetMethods(Flags))
                {
                    AddRef(refTypes, method.ReturnType);
                    foreach (var param in method.GetParameters())
                    {
                        AddRef(refTypes, param.ParameterType);
                    }
                }
            }

            var allowVoid = AllowArgumentsLength("UsingAction");
            var allowReturn = AllowArgumentsLength("UsingFunc");
            var genInfos = new List<GenInfo>();
            foreach (var type in refTypes.Distinct())
            {
                if (ContainsValueParameter(type))
                {
                    var info = ToGenInfo(type);
                    if (info.HasReturn && !allowReturn.Contains(info.Parameters.Length) || !info.HasReturn && !allowVoid.Contains(info.Parameters.Length))
                    {
                        UnityEngine.Debug.LogWarning(string.Format("Method parameter length don't match:{0} \nSource  type: {1} ", info, type.ToString()));
                        continue;
                    }
                    if (genInfos.Contains(info))
                        continue;
                    genInfos.Add(info);
                }
            }
            using (var jsEnv = new JsEnv())
            {
                var autoRegisterRender = jsEnv.Eval<Func<GenInfo[], string>>("require('puerts/templates/wrapper-reg-using.tpl.cjs')");
                using (StreamWriter textWriter = new StreamWriter(saveTo + "AutoStaticCodeUsing.cs", false, Encoding.UTF8))
                {
                    string fileContext = autoRegisterRender(genInfos.OrderBy(o => (o.Name + "<" + string.Join(", ", o.Parameters))).ToArray());
                    textWriter.Write(fileContext);
                    textWriter.Flush();
                }
            }
        }
    }
}
