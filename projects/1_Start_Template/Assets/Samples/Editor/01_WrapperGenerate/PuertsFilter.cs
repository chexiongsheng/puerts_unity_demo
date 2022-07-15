using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Puerts;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Puerts.Editor.Generator;

/// <summary>
/// 这是参照xLua黑名单配置设计的
/// </summary>
[Configure]
public class PuertsFilter : Editor
{
    /// <summary>
    /// 配置黑名单字段丶方法
    /// 参数:
    /// 1:类型名称, 2:方法或字段名称, 3及以后:方法参数类型名称
    /// </summary>
    public static List<List<string>> BlackList
    {
        get
        {
            return new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
        #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
        #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "shadowRadius"},
                new List<string>(){"UnityEngine.Light", "shadowAngle"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.Light", "SetLightDirty"},
                new List<string>(){"UnityEngine.LightProbeGroup", "dering"},
                new List<string>(){"UnityEngine.LightProbeGroup", "probePositions"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
        #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
        #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                new List<string>(){"UnityEngine.AnimatorControllerParameter", "name"},
                new List<string>(){"UnityEngine.AudioSettings", "GetSpatializerPluginNames"},
                new List<string>(){"UnityEngine.AudioSettings", "SetSpatializerPluginName", "System.String"},
                new List<string>(){"UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame"},
                new List<string>(){"UnityEngine.Input", "IsJoystickPreconfigured", "System.String"},
                new List<string>(){"UnityEngine.ParticleSystemForceField", "FindAll"},
                new List<string>(){"UnityEngine.Texture", "imageContentsHash"},
                new List<string>(){"UnityEngine.UI.Graphic", "OnRebuildRequested"},
                new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
                new List<string>(){"UnityEngine.DrivenRectTransformTracker", "StartRecordingUndo"},
                new List<string>(){"UnityEngine.DrivenRectTransformTracker", "StopRecordingUndo"},
                new List<string>(){"UnityEngine.Terrain", "bakeLightProbesForTrees"},
                new List<string>(){"UnityEngine.Terrain", "deringLightProbesForTrees"},
                new List<string>(){"UnityEngine.GUIStyleState", "scaledBackgrounds"},
                new List<string>(){"UnityEngine.Caching", "SetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                new List<string>(){"UnityEngine.Caching", "SetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
                new List<string>(){"UnityEngine.Caching", "ResetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                new List<string>(){"UnityEngine.Caching", "ResetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
#if UNITY_ANDROID
                new List<string>(){"UnityEngine.Handheld", "SetActivityIndicatorStyle", "UnityEngine.iOS.ActivityIndicatorStyle"},
#endif
#if UNITY_IOS
                new List<string>(){"UnityEngine.Handheld", "SetActivityIndicatorStyle", "UnityEngine.AndroidActivityIndicatorStyle"},
#endif
                //System.IO
                new List<string>(){"System.IO.FileInfo", "GetAccessControl"},
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.Directory", "GetAccessControl", "System.String"},
                new List<string>(){"System.IO.Directory", "GetAccessControl", "System.String", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.Directory", "SetAccessControl", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.Directory", "CreateDirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.File", "SetAccessControl", "System.String", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.File", "GetAccessControl", "System.String"},
                new List<string>(){"System.IO.File", "GetAccessControl", "System.String", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.File", "Create", "System.String", "System.Int32", "System.IO.FileOptions", "System.Security.AccessControl.FileSecurity"},
                //System.Stream
                new List<string>(){"System.IO.FileStream", ".ctor", "System.String", "System.IO.FileMode", "System.Security.AccessControl.FileSystemRights", "System.IO.FileShare", "System.Int32", "System.IO.FileOptions"},
                new List<string>(){"System.IO.FileStream", ".ctor", "System.String", "System.IO.FileMode", "System.Security.AccessControl.FileSystemRights", "System.IO.FileShare", "System.Int32", "System.IO.FileOptions", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.FileStream", "GetAccessControl"},
                new List<string>(){"System.IO.FileStream", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.Stream", "Read", "System.Span<System.Byte>"},
                new List<string>(){"System.IO.Stream", "Write", "System.ReadOnlySpan<System.Byte>"},
                new List<string>(){"System.IO.Stream", "ReadAsync", "System.Memory<System.Byte>"},
                new List<string>(){"System.IO.Stream", "ReadAsync", "System.Memory<System.Byte>", "System.Threading.CancellationToken"},
                new List<string>(){"System.IO.Stream", "WriteAsync", "System.ReadOnlyMemory<System.Byte>"},
                new List<string>(){"System.IO.Stream", "WriteAsync", "System.ReadOnlyMemory<System.Byte>", "System.Threading.CancellationToken"},
                //System.Type
                new List<string>(){"System.Type", "MakeGenericSignatureType", "System.Type", "System.Type[]" },
                new List<string>(){"System.Type", "IsCollectible" },
                //System
                new List<string>(){"System.Net.WebProxy", "CreateDefaultProxy" },
                new List<string>(){"System.Threading.Thread", "CurrentContext"},
                new List<string>(){"System.MarshalByRefObject", "CreateObjRef", "System.Type"},
                new List<string>(){"System.Reflection.FieldInfo", "GetValueDirect", "System.TypedReference"},
                new List<string>(){"System.Reflection.FieldInfo", "SetValueDirect", "System.TypedReference", "System.Object"},
                new List<string>(){"System.Reflection.IntrospectionExtensions", "GetTypeInfo", "System.Type"},
                new List<string>(){"NUnit.Compatibility.AdditionalTypeExtensions", "IsCastableFrom", "System.Type", "System.Type"},
                //Puerts
                new List<string>(){"Puerts.JsEnv", "debugPort"},
                new List<string>(){"Puerts.JsEnv", "OnJsEnvCreate"},
                new List<string>(){"Puerts.JsEnv", "OnJsEnvDispose"},

            };
        }
    }

    [Filter]
    //适用于puerts_1.3.0版本及以上
    static BindingMode Filter(MemberInfo memberInfo) => (FilterObsolete(memberInfo) || FilterByBlackList(memberInfo)) ? BindingMode.DontBinding : BindingMode.FastBinding;
    //适用于puerts_1.3.0版本以下
    //static bool Filter(MemberInfo memberInfo) => FilterObsolete(memberInfo) || FilterByBlackList(memberInfo);

    static bool FilterObsolete(MemberInfo memberInfo)
    {
        var obsolete = memberInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).FirstOrDefault() as ObsoleteAttribute;
        if (obsolete != null)
        {
            return obsolete.IsError;   //过滤Obsolete-Error属性时
        }
        return false;
    }
    static bool FilterByBlackList(MemberInfo memberInfo)
    {
        string declaringTypeName = memberInfo.DeclaringType.FullName.Replace("+", ".");
        Dictionary<string, List<string[]>> methodOrProp;
        List<string[]> paramtersList;
        if (blacklist.TryGetValue(declaringTypeName, out methodOrProp) && (
            methodOrProp.TryGetValue(memberInfo.Name, out paramtersList) ||
            memberInfo is ConstructorInfo && methodOrProp.TryGetValue(memberInfo.DeclaringType.Name, out paramtersList)
        ))
        {
            //如果是字段声明, 直接返回
            if (!(memberInfo is MethodInfo || memberInfo is ConstructorInfo))
                return true;

            var paramters = memberInfo is MethodInfo ? ((MethodInfo)memberInfo).GetParameters() : ((ConstructorInfo)memberInfo).GetParameters();
            var paramterNames = (from pInfo in paramters select GetFriendlyName(pInfo.ParameterType)).ToArray();

            if (IsMatch(paramtersList, paramterNames))
                return true;
            for (var i = paramterNames.Length - 1; i >= 0; i--)
            {
                if (!paramters[i].IsOptional)
                    break;
                if (IsMatch(paramtersList, paramterNames.Take(i).ToArray()))
                    return true;
            }
        }

        //过滤扩展方法
        if (memberInfo is MethodInfo && ((MethodInfo)memberInfo).IsStatic && memberInfo.IsDefined(typeof(ExtensionAttribute)))
        {
            var mParamters = ((MethodInfo)memberInfo).GetParameters();
            var mParamterNames = (from pInfo in mParamters select GetFriendlyName(pInfo.ParameterType)).ToArray();

            declaringTypeName = mParamterNames[0];
            mParamters = mParamters.Skip(1).ToArray();
            mParamterNames = mParamterNames.Skip(1).ToArray();
            if (blacklist.TryGetValue(declaringTypeName, out methodOrProp) && methodOrProp.TryGetValue(memberInfo.Name, out paramtersList))
            {

                if (IsMatch(paramtersList, mParamterNames))
                    return true;
                for (var i = mParamterNames.Length - 1; i >= 0; i--)
                {
                    if (!mParamters[i].IsOptional)
                        break;
                    if (IsMatch(paramtersList, mParamterNames.Take(i).ToArray()))
                        return true;
                }
            }
        }
        return false;
    }
    static bool IsMatch(List<string[]> paramtersList, string[] mParamters)
    {
        foreach (var paramters in paramtersList)
        {
            //*(通配) / 屏蔽所有此名称的方法
            if (paramters.Length == 1 && paramters[0] == "*")
                return true;
            //对比方法参数
            if (paramters.Length == mParamters.Length)
            {
                var exclude = true;
                for (int i = 0; i < paramters.Length && exclude; i++)
                {
                    if (paramters[i] != mParamters[i])
                        exclude = false;
                }
                if (exclude)
                    return true;
            }
        }

        return false;
    }
    static string GetNameWithoutNamespace(Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgumentNames = type.GetGenericArguments()
                .Select(x => GetFriendlyName(x)).ToArray();
            return type.Name.Split('`')[0] + "<" + string.Join(",", genericArgumentNames) + ">";
        }
        else
        {
            return type.Name;
        }
    }
    static string GetFriendlyName(Type type)
    {
        if (string.IsNullOrEmpty(type.FullName))
            return string.Empty;

        if (type.IsArray)
        {
            if (type.GetArrayRank() > 1)
            {
                return GetFriendlyName(type.GetElementType()) + "[" + new String(',', type.GetArrayRank() - 1) + "]";
            }
            else
            {
                return GetFriendlyName(type.GetElementType()) + "[]";
            }
        }
        else if (type.IsNested)
        {
            if (type.DeclaringType.IsGenericTypeDefinition)
            {
                var genericArgumentNames = type.GetGenericArguments()
                    .Select(x => GetFriendlyName(x)).ToArray();
                return type.DeclaringType.FullName.Split('`')[0] + "<" + string.Join(",", genericArgumentNames) + ">" + '.' + type.Name;
            }
            else
            {
                return GetFriendlyName(type.DeclaringType) + '.' + GetNameWithoutNamespace(type);
            }
        }
        else if (type.IsGenericType)
        {
            var genericArgumentNames = type.GetGenericArguments()
                .Select(x => GetFriendlyName(x)).ToArray();
            return type.FullName.Split('`')[0] + "<" + string.Join(",", genericArgumentNames) + ">";
        }
        return type.FullName.Replace("+", ".");
    }

    static Dictionary<string, Dictionary<string, List<string[]>>> _blacklist;
    static Dictionary<string, Dictionary<string, List<string[]>>> blacklist
    {
        get
        {
            if (_blacklist == null)
            {
                _blacklist = new Dictionary<string, Dictionary<string, List<string[]>>>();

                foreach (var memberInfo in BlackList)
                {
                    if (memberInfo.Count < 2)
                        continue;
                    var _memberInfo = memberInfo.Select(o => o.Replace(" ", "")).ToList();

                    string fullname = _memberInfo[0];
                    string methodName = _memberInfo[1];
                    Dictionary<string, List<string[]>> methodOrProp;
                    if (!_blacklist.TryGetValue(fullname, out methodOrProp))
                    {
                        methodOrProp = new Dictionary<string, List<string[]>>();
                        _blacklist.Add(fullname, methodOrProp);
                    }
                    List<string[]> paramtersList;
                    if (!methodOrProp.TryGetValue(methodName, out paramtersList))
                    {
                        paramtersList = new List<string[]>();
                        methodOrProp.Add(methodName, paramtersList);
                    }
                    paramtersList.Add(_memberInfo.GetRange(2, _memberInfo.Count - 2).ToArray());
                }
            }
            return _blacklist;
        }
    }
}