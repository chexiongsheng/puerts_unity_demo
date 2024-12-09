using UnityEditor;
using UnityEngine;
using Puerts;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
class NodeTSCAndHotReload
{
    static Puerts.Editor.NodeRunner runner;
    public static Action<int> addDebugger;
    public static Action<int> removeDebugger;

    static NodeTSCAndHotReload()
    {
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
        if (runner != null && removeDebugger != null && envAndPort.TryGetValue(env, out debugPort)) {
            removeDebugger(debugPort);
        }
    }

    static void BeforeAssemblyReload()
    {
        if (runner != null) 
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
        Puerts.Editor.NodeRunner runner1 = new Puerts.Editor.NodeRunner();
        runner1.Run("require('./build-script/compile-and-move.js')");
    }
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Compile TsProj", true)] 
    static bool CompileValidate() 
    {
        var env = new JsEnv();
        return env.Backend is BackendNodeJS;
    }

    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/on")]
    static void Watch() 
    {
        runner = new Puerts.Editor.NodeRunner();
        try 
        {
            runner.env.UsingAction<int>();
            runner.Run(@"require('./build-script/watch-and-hotreload.js');");

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
        return PuertsDLL.GetLibBackend() == 1 && runner == null;
    }

    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/off")]
    static void UnWatch() 
    {
        runner = null;
        addDebugger = null;
        removeDebugger = null;
        UnityEngine.Debug.Log("stop watching tsproj");
    }
    [MenuItem("PuertsEditorDemo/tsc & HotReload/Watch tsProj And HotReload/off", true)]
    static bool UnWatchValidate() 
    {
        return PuertsDLL.GetLibBackend() == 1 && runner != null && runner.env != null;
    }
}