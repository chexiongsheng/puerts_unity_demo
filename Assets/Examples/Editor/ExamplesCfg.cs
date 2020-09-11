/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System.Collections.Generic;
using Puerts;
using System;
using UnityEngine;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

//1、配置类必须打[Configure]标签
//2、必须放Editor目录
[Configure]
public static class ExamplesCfg
{
    [Binding]
    static IEnumerable<Type> Bindings
    {
        get
        {
            return new List<Type>()
            {
                typeof(Debug),
                typeof(PuertsTest.TestClass),
                typeof(Vector3),
                typeof(List<int>),
                //typeof(Dictionary<string, int>),
                typeof(PuertsTest.BaseClass),
                typeof(PuertsTest.DerivedClass),
                typeof(PuertsTest.MyEnum),
                typeof(Time),
                typeof(Transform),
                typeof(Component),
                typeof(GameObject),
                typeof(UnityEngine.Object),
                typeof(Delegate),
                typeof(System.Object),
                typeof(Type),
                typeof(ParticleSystem),
                typeof(Canvas),
                typeof(RenderMode),
                typeof(Behaviour),
                typeof(MonoBehaviour),

                typeof(UnityEngine.EventSystems.UIBehaviour),
                typeof(UnityEngine.UI.Selectable),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.UI.Button.ButtonClickedEvent),
                typeof(UnityEngine.Events.UnityEvent),
                typeof(UnityEngine.UI.InputField),
                typeof(UnityEngine.UI.Toggle),
                typeof(UnityEngine.UI.Toggle.ToggleEvent),
                typeof(UnityEngine.Events.UnityEvent<bool>),

            };
        }
    }
    [BlittableCopy]
    static IEnumerable<Type> Blittables
    {
        get
        {
            return new List<Type>()
            {
                //打开这个可以优化Vector3的GC，但需要开启unsafe编译
                //typeof(Vector3),
            };
        }
    }

    [Binding]
    static IEnumerable<Type> DynamicBindings
    {
        get
        {
            var namespaces = new List<string>()  // 在这里添加名字空间
            {
                "System",
                "System.IO",
                //Unity Api
                "UnityEngine",
                "UnityEngine.UI",
            };
            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                              from type in assembly.GetExportedTypes()
                              where type.Namespace != null && namespaces.Contains(type.Namespace) && !IsExcluded(type)
                                      && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                              select type);

            string[] customAssemblys = new string[] {
                "Assembly-CSharp",
            };
            var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                               from type in assembly.GetExportedTypes()
                               where type.Namespace == null || !type.Namespace.StartsWith("Puerts")
                                       && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                               select type);

            return unityTypes.Concat(customTypes);
        }
    }
    static List<string> exclude = new List<string>
    {
        "HideInInspector", "ExecuteInEditMode",
        "AddComponentMenu", "ContextMenu",
        "RequireComponent", "DisallowMultipleComponent",
        "SerializeField", "AssemblyIsEditorAssembly",
        "Attribute", "Types",
        "UnitySurrogateSelector", "TrackedReference",
        "TypeInferenceRules", "FFTWindow",
        "RPC", "Network", "MasterServer",
        "BitStream", "HostData",
        "ConnectionTesterStatus", "GUI", "EventType",
        "EventModifiers", "FontStyle", "TextAlignment",
        "TextEditor", "TextEditorDblClickSnapping",
        "TextGenerator", "TextClipping", "Gizmos",
        "ADBannerView", "ADInterstitialAd",
        "Android", "Tizen", "jvalue",
        "iPhone", "iOS", "Windows", "CalendarIdentifier",
        "CalendarUnit", "CalendarUnit",
        "ClusterInput", "FullScreenMovieControlMode",
        "FullScreenMovieScalingMode", "Handheld",
        "LocalNotification", "NotificationServices",
        "RemoteNotificationType", "RemoteNotification",
        "SamsungTV", "TextureCompressionQuality",
        "TouchScreenKeyboardType", "TouchScreenKeyboard",
        "MovieTexture", "UnityEngineInternal",
        "Terrain", "Tree", "SplatPrototype",
        "DetailPrototype", "DetailRenderMode",
        "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
        "SendMouseEvents", "Cursor", "Flash", "ActionScript",
        "OnRequestRebuild", "Ping",
        "ShaderVariantCollection", "SimpleJson.Reflection",
        "CoroutineTween", "GraphicRebuildTracker",
        "Advertisements", "UnityEditor", "WSA",
        "EventProvider", "Apple",
        "ClusterInput", "Motion",
        "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
        "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",

        "U2018Compatible", "UnityEditor", "AppDomain", "System.Activator",
        "System.ArgIterator", "System.Exception", "System.ComponentModel",
        "System.IntPtr", "System.UIntPtr", "System.IO.Stream", "System.IO.UnmanagedMemoryStream",
        "System.ActivationContext", "System.ApplicationIdentity", "System.MarshalByRefObject",
        "System.SpanExtensions", "System.TypedReference", "System.Reflection.Module",
        "System.Reflection.Pointer", "System.Reflection.Assembly",
        "System.Reflection.FieldInfo", "System.Reflection.MethodInfo", "System.Reflection.TypeDelegator",
        "System.Reflection.ICustomTypeProvider", "System.Security.IStackWalk", "System.Security.IEvidenceFactory",
        "System.Security.NamedPermissionSet", "System.Security.PermissionSet", "System.Security.SecureString",
        "System.Security.SecurityState", "System.Security.SecurityContext", "System.Security.ReadOnlyPermissionSet",
        "System.Security.ISecurityPolicyEncodable", "System.Security.HostSecurityManager", "System.Security.CodeAccessPermission",

        "System.Text.Encoder", "System.Text.Decoder", "System.Text.Encoding", "System.Text.UTF7Encoding", "System.Text.UTF8Encoding",
        "System.Text.UTF32Encoding", "System.Text.UnicodeEncoding", "System.Text.StringBuilder",
        "System.Tuple", "System.Byte", "System.Int16", "System.Int32", "System.Int64",
        "System.Single", "System.Double", "System.String", "System.Buffer", "System.Decimal",

        "UnityEngine.UI.IMask", "UnityEngine.GUIElement", "UnityEngine.GUILayer",
        "UnityEngine.GUIText", "UnityEngine.GUITexture", "UnityEngine.HostData",
        "UnityEngine.TerrainData", "UnityEngine.RemoteNotification", "UnityEngine.LocalNotification",
        "UnityEngine.MasterServer", "UnityEngine.iPhone", "UnityEngine.iPhoneTouch",
        "UnityEngine.iPhoneKeyboard", "UnityEngine.iPhoneInput", "UnityEngine.Vector3Int",
        "UnityEngine.Vector2Int", "UnityEngine.BitStream", "UnityEngine.ADBannerView",
        "UnityEngine.ADInterstitialAd", "UnityEngine.ParticleSystem.CollisionEvent",
        "UnityEngine.ProceduralPropertyDescription", "UnityEngine.ProceduralMaterial",
        "UnityEngine.ProceduralTexture", "UnityEngine.AnimationInfo",
        "UnityEngine.HashUnsafeUtilities", "UnityEngine.ParticleSystem.CollisionEvent",
        "UnityEngine.UI.BaseVertexEffect",
    };
    static bool IsExcluded(Type type)
    {
        var fullname = type.FullName.Replace("+", ".");
        for (int i = 0; i < exclude.Count; i++)
        {
            if (fullname.Contains(exclude[i]))
            {
                return true;
            }
        }
        if (type.BaseType != null)
            return IsExcluded(type.BaseType);

        return false;
    }

    [Blacklist]
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
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
        #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
        #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
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
}
