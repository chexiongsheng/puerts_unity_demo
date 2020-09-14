using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using System.Diagnostics;

[InitializeOnLoad]
public static class TsUtils
{
    /// <summary>
    /// Ts项目根目录
    /// </summary>
    static string root = Path.GetDirectoryName(Application.dataPath) + "/TsProj";
    /// <summary>
    /// Compile编译回调路径, 必须在Assets目录之下
    /// </summary>
    static string compile = Application.dataPath + "/Editor/UnityCompile.cs";

    static FileSystemWatcher watcher;
    static bool IsChanged = false;
    static bool IsCompile
    {
        get { return EditorPrefs.GetBool("_Editor.TScriptCompile", false); }
        set { EditorPrefs.SetBool("_Editor.TScriptCompile", value); }
    }
    static bool IsUpdate
    {
        get { return EditorPrefs.GetBool("_Editor.TScriptUpdate", true); }
        set { EditorPrefs.SetBool("_Editor.TScriptUpdate", value); }
    }

    static TsUtils()
    {
        Watcher();
    }

    [MenuItem("Tools/TScripts/Enable Compile")]
    static void EnableCompile()
    {
        IsCompile = true;
        IsUpdate = true;
        Watcher();
    }

    [MenuItem("Tools/TScripts/Enable Compile", true)]
    static bool EnableCompileCheck()
    {
        return !IsCompile;
    }

    [MenuItem("Tools/TScripts/Disable Compile")]
    static void DisableCompile()
    {
        IsCompile = false;
        RemoveWatcher();
    }

    [MenuItem("Tools/TScripts/Disable Compile", true)]
    static bool DisableCompileCheck()
    {
        return IsCompile;
    }
    [MenuItem("Tools/TScripts/Tsc Compile")]
    public static void TscCompile()
    {
        var args = "-p " + root;
        RunCmd("tsc", args, true, null);
        //Refresh
        AssetDatabase.Refresh();
    }

    static void RemoveWatcher()
    {
        EditorApplication.update -= Update;
        EditorApplication.delayCall -= DelayCall;
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;
        }
    }
    static void Watcher()
    {
        RemoveWatcher();
        if (!IsCompile)
            return;

        //Debug.Log("Watcher: " + root);

        watcher = new FileSystemWatcher();
        try { watcher.Path = root; }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
            return;
        }
        //监听子文件夹
        watcher.IncludeSubdirectories = true;
        //监听事件类型
        watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite
            | NotifyFilters.Size;// | NotifyFilters.Attributes | NotifyFilters.CreationTime;
        //过滤后缀名
        watcher.Filter = "*.ts";
        //回调方法
        Action<object, FileSystemEventArgs> changed = (source, e) => IsChanged = true;
        Action<object, RenamedEventArgs> renamed = (source, e) => IsChanged = true;
        watcher.Changed += new FileSystemEventHandler(changed);
        watcher.Created += new FileSystemEventHandler(changed);
        watcher.Deleted += new FileSystemEventHandler(changed);
        watcher.Renamed += new RenamedEventHandler(renamed);

        //注册事件
        watcher.EnableRaisingEvents = true;
        EditorApplication.update += Update;
        EditorApplication.delayCall += DelayCall;

        //Tsc编译
        DelayCall();
    }
    static void DelayCall()
    {
        //File.Delete(compile);
        if (!IsUpdate)
            return;

        IsUpdate = false;
        TscCompile();
    }
    //这里必须在Unity主线程调用(FileSystemWatcher回调在其他线程)
    static void Update()
    {
        if (!IsChanged)
            return;

        IsChanged = false;
        IsUpdate = true;

        var random = new System.Random().Next(int.MaxValue);
        var content = "public class UnityCompile" + random + " { int random = " + random + "; }";
        Directory.CreateDirectory(Path.GetDirectoryName(compile));
        File.WriteAllText(compile, content);
    }

    public static void RunCmd(string command, string args, bool shell, string workdir)
    {
        ProcessStartInfo info = new ProcessStartInfo(command);
        info.Arguments = args;
        //不显示窗口
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        //使用Shell
        info.UseShellExecute = shell;
        //重定向输入/输出/输出错误(Shell不能重定向)
        info.RedirectStandardOutput = !info.UseShellExecute;
        info.RedirectStandardError = !info.UseShellExecute;
        info.RedirectStandardInput = !info.UseShellExecute;
        //编码格式
        if (!info.UseShellExecute)
        {
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
        //工作区域
        if (!string.IsNullOrEmpty(workdir))
            info.WorkingDirectory = workdir;

        Debug.Log($"{info.WorkingDirectory}> {command} {args}");

        Process process = Process.Start(info);
        if (!info.UseShellExecute)
        {
            var output = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
                Debug.Log($"{info.WorkingDirectory}> {output}");
            var err = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(err))
                Debug.LogError($"{info.WorkingDirectory}> {err}");
        }
        process.WaitForExit();
        process.Close();
    }
}