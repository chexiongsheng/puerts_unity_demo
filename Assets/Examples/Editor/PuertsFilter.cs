using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Puerts;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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
#if UNITY_ANDROID
                new List<string>(){"UnityEngine.Handheld", "SetActivityIndicatorStyle", "UnityEngine.iOS.ActivityIndicatorStyle"},
#endif
#if UNITY_IOS
                new List<string>(){"UnityEngine.Handheld", "SetActivityIndicatorStyle", "UnityEngine.AndroidActivityIndicatorStyle"},
#endif
                //System.Reflection
                new List<string>(){"System.Reflection.FieldInfo", "GetValueDirect", "System.TypedReference"},
                new List<string>(){"System.Reflection.FieldInfo", "SetValueDirect", "System.TypedReference", "System.Object"},
                new List<string>(){"System.Reflection.IntrospectionExtensions", "GetTypeInfo", "System.Type"},
                new List<string>(){"NUnit.Compatibility.AdditionalTypeExtensions", "IsCastableFrom", "System.Type", "System.Type"},
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
            };
        }
    }

    /*/过滤Obsolete-Error属性时, 放开此行注释
    [Filter]
    static bool FilterObsolete(MemberInfo memberInfo)
    {
        var obsolete = memberInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).FirstOrDefault() as ObsoleteAttribute;
        return obsolete != null && obsolete.IsError;
    }//*/

    [Filter]
    static bool Filter(MemberInfo memberInfo)
    {
        string declaringTypeName = memberInfo.DeclaringType.FullName.Replace("+", ".");
        Dictionary<string, List<string[]>> methodOrProp;
        List<string[]> paramtersList;
        if (blacklist.TryGetValue(declaringTypeName, out methodOrProp) && methodOrProp.TryGetValue(memberInfo.Name, out paramtersList))
        {
            //如果是字段声明, 直接返回
            if (!(memberInfo is MethodInfo))
                return true;

            var mParamters = ((MethodInfo)memberInfo).GetParameters();
            var mParamterNames = (from pInfo in mParamters
                                  let pTypeName = pInfo.ParameterType.FullName
                                  select pTypeName != null ? pTypeName.Replace("+", ".") : "").ToArray();

            if (IsMatch(paramtersList, mParamterNames))
                return true;
            for (var i = mParamterNames.Length - 1; i >= 0; i++)
            {
                if (!mParamters[i].IsOptional)
                    break;
                if (IsMatch(paramtersList, mParamterNames.Take(i).ToArray()))
                    return true;
            }
        }

        //过滤扩展方法
        if (memberInfo is MethodInfo && ((MethodInfo)memberInfo).IsStatic && memberInfo.IsDefined(typeof(ExtensionAttribute)))
        {
            var mParamters = ((MethodInfo)memberInfo).GetParameters();
            var mParamterNames = (from pInfo in mParamters
                                  let pTypeName = pInfo.ParameterType.FullName
                                  select pTypeName != null ? pTypeName.Replace("+", ".") : "").ToArray();

            declaringTypeName = mParamterNames[0];
            mParamters = mParamters.Skip(1).ToArray();
            mParamterNames = mParamterNames.Skip(1).ToArray();
            if (blacklist.TryGetValue(declaringTypeName, out methodOrProp) && methodOrProp.TryGetValue(memberInfo.Name, out paramtersList))
            {

                if (IsMatch(paramtersList, mParamterNames))
                    return true;
                for (var i = mParamterNames.Length - 1; i >= 0; i++)
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


    static Dictionary<string, Dictionary<string, List<string[]>>> _blacklist;
    static Dictionary<string, Dictionary<string, List<string[]>>> blacklist
    {
        get
        {
            if (_blacklist == null)
            {
                _blacklist = new Dictionary<string, Dictionary<string, List<string[]>>>();

                foreach (var blackInfo in BlackList)
                {
                    if (blackInfo.Count < 2)
                        continue;

                    string fullname = blackInfo[0];
                    string methodName = blackInfo[1];
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
                    paramtersList.Add(blackInfo.GetRange(2, blackInfo.Count - 2).ToArray());
                }
            }
            return _blacklist;
        }
    }
}
