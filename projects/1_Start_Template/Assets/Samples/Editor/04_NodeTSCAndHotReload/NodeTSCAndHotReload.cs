using UnityEditor;
using UnityEngine;
using Puerts;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
class NodeTSCAndHotReload
{
    static JsEnv env;
    public static Action<int> addDebugger;
    public static Action<int> removeDebugger;

    static NodeTSCAndHotReload()
    {
        EditorApplication.update += Update;
        JsEnv.OnJsEnvCreate += OnJsEnvCreate;
        JsEnv.OnJsEnvDispose += OnJsEnvDispose;

        AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;

        if (EditorPrefs.GetBool("NodeTSCAndHotReload.justShutdownByReload")) 
        {
            EditorPrefs.SetBool("NodeTSCAndHotReload.justShutdownByReload", false);
            Watch();
        }
    }

    // 记录存在的所有JSEnv及其debugPort
    static Dictionary<JsEnv, int> envAndPort = new Dictionary<JsEnv, int>();
    static void OnJsEnvCreate(JsEnv env, ILoader loader, int debugPort)
    {
        if (debugPort != -1 && env != null && addDebugger != null) {  
            UnityEngine.Debug.Log("OnJsEnvCreate:" + debugPort);
            envAndPort.Add(env, debugPort);
            addDebugger(debugPort);
        }
    }
    static void OnJsEnvDispose(JsEnv env)
    {
        int debugPort = 0;
        if (env != null && removeDebugger != null && envAndPort.TryGetValue(env, out debugPort)) {
            removeDebugger(debugPort);
        }
    }

    static void Update()
    {
        if (env != null)
        {
            env.Tick();
        }
    }

    static void BeforeAssemblyReload()
    {
        if (env != null) 
        {
            EditorPrefs.SetBool("NodeTSCAndHotReload.justShutdownByReload", true);
            UnWatch();
        } 
    }   

    [MenuItem("PuertsEditorDemo/tsc & HotReload/__TIPS__", false, 10)] 
    static void readme() {
        EditorUtility.DisplayDialog("tips", @"
你可以使用这个功能来编译 TsProject/src 目录的typescript。
并自动将改动热重载至游戏中（需要打开inspector）
但请确认你使用的是Node版本puerts
并且已经在 Puer-Project 目录执行`npm i`

You can use this feature to compile typescript in TsProject/src directory.
It will hot reload the new code in the game with inspector opened.
But please confirm that you're using the Node version Puerts 
and run `npm i` in Puer-Project
        ", "ok");
    }
 
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Compile TsProj")] 
    static void Compile() 
    {
        JsEnv env = Puerts.Editor.NodeJS.RunInPuerProject(@"
            require('./build-script/compile-and-move.js');
        ");
        env.Dispose();
        env = null;
    }
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Compile TsProj", true)] 
    static bool CompileValidate() 
    {
        return PuertsDLL.GetLibBackend() == 1;
    }

    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/on")]
    static void Watch() 
    {
        env = new JsEnv();
        env.UsingAction<int>();
        try 
        {
            Puerts.Editor.NodeJS.RunInPuerProject(@"
                require('./build-script/watch-and-hotreload.js');
            ", env);

            UnityEngine.Debug.Log("watching tsproj");
        } 
        catch(Exception e)
        {
            UnWatch();
            throw e;
        }
    }
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/on", true)]
    static bool WatchValidate() 
    {
        return PuertsDLL.GetLibBackend() == 1 && env == null;
    }

    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/off")]
    static void UnWatch() 
    {
        env.Dispose();
        env = null;
        addDebugger = null;
        removeDebugger = null;
        UnityEngine.Debug.Log("stop watching tsproj");
    }
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/off", true)]
    static bool UnWatchValidate() 
    {
        return PuertsDLL.GetLibBackend() == 1 && env != null;
    }
}