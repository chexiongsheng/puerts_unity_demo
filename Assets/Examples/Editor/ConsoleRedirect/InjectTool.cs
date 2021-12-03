using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

/// <summary>
/// (Unity)已验证版本: 2019.4.28
/// </summary>
public static class InjectTool
{
    static string AssemblyPath = "";
    static string BackupPath { get { return AssemblyPath + ".backup"; } }
    static string InjectPath { get { return AssemblyPath + ".inject"; } }

    const string InjectType = "UnityEditor.ConsoleWindow";
    const string HyperLinkClickedMethod = "EditorGUI_HyperLinkClicked";

    public static void Inject()
    {
        _InitAssemblyPath();

        if (string.IsNullOrEmpty(AssemblyPath) || !File.Exists(AssemblyPath))
        {
            Debug.LogError($"File don't exist: ${AssemblyPath}");
            return;
        }

        try
        {
            _Inject();

            Debug.Log("Inject Completed.");

            bool openFolder = EditorUtility.DisplayDialog("提示",
@"如何替换UnityEditor.dll文件?

1. 关闭Unity进程
2. 删除UnityEditor.dll文件
3. 将UnityEditor.dll.inject文件重命名为UnityEditor.dll
4. 重新运行Unity", "Folder", "Cancel");

            if (openFolder) OpenDLLFolder();
        }
        catch (Exception e)
        {
            Debug.LogError($"Inject Fail: {e}");
        }
    }
    public static void RestoreOriginal()
    {
        _InitAssemblyPath();

        if (string.IsNullOrEmpty(AssemblyPath) || !File.Exists(AssemblyPath))
        {
            Debug.LogError($"File don't exist: ${AssemblyPath}");
            return;
        }
        if (!File.Exists(BackupPath))
        {
            Debug.LogError($"File don't exist: ${BackupPath}");
            return;
        }

        bool openFolder = EditorUtility.DisplayDialog("提示",
@"如何还原UnityEditor.dll文件?

1. 关闭Unity进程
2. 删除UnityEditor.dll文件
3. 将UnityEditor.dll.backup文件重命名为UnityEditor.dll
4. 重新运行Unity", "Folder");

        if (openFolder) OpenDLLFolder();
    }
    public static void OpenDLLFolder()
    {
        _InitAssemblyPath();

        if (string.IsNullOrEmpty(AssemblyPath) || !File.Exists(AssemblyPath))
        {
            Debug.LogError($"File don't exist: ${AssemblyPath}");
            return;
        }
        EditorUtility.RevealInFinder(AssemblyPath);
    }

    static void _InitAssemblyPath()
    {
        if (string.IsNullOrEmpty(AssemblyPath) || !File.Exists(AssemblyPath))
        {
            AssemblyPath = null;

            string assemblyName = "UnityEditor.dll";
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                Type type = assembly.GetType(InjectType, false);
                if (type == null) continue;

                string path = type.Assembly.Location;
                if (path != null && path.EndsWith(assemblyName))
                {
                    AssemblyPath = path;
                    break;
                }
            }
            Debug.Log("Load AssemblyPath: " + AssemblyPath);
        }
    }

    static void _Inject()
    {
        if (!File.Exists(BackupPath))
        {
            File.WriteAllBytes(BackupPath, File.ReadAllBytes(AssemblyPath));
            Debug.Log($"AssemblyPath backup: {BackupPath}");
        }
        if (File.Exists(InjectPath))
        {
            File.Delete(InjectPath);
        }

        DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(Path.GetDirectoryName(BackupPath));
        AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(BackupPath, new ReaderParameters
        {
            ReadWrite = true,
            ReadSymbols = false,
            ReadingMode = ReadingMode.Immediate,
            AssemblyResolver = resolver,
        });

        try
        {
            ModuleDefinition module = assembly.MainModule;
            TypeDefinition type = module.GetType(InjectType);
            if (type == null)
            {
                throw new Exception($"Not found type `{InjectType}`");
            }

            _Inject_LinkClickedMethod(module, type.Methods.First(m => m.Name == HyperLinkClickedMethod));
            //_Inject_TestMethod(module, type.Methods.ToArray());

            assembly.Write(InjectPath);
        }
        finally
        {
            if (assembly.MainModule.SymbolReader != null)
            {
                assembly.MainModule.SymbolReader.Dispose();
            }
        }
    }

    static void _Inject_LinkClickedMethod(ModuleDefinition module, MethodDefinition method)
    {
        if (method == null)
        {
            throw new Exception($"Not found method `{InjectType}.{HyperLinkClickedMethod }`");
        }
        MethodInfo onLinkClicked = typeof(UnityEditor.Console.OnClicked).GetMethod("OnLinkClicked", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (onLinkClicked == null)
            throw new Exception("Not found inject method");

        MethodReference injMethod = module.ImportReference(onLinkClicked);

        ILProcessor ilProcessor = method.Body.GetILProcessor();
        Instruction firstPoint = method.Body.Instructions[0];

        //需要用到的类型
        TypeReference ObjectType = module.ImportReference(typeof(System.Object));

        ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Nop));
        //取出EditorGUI_HyperLinkClicked的参数, 封装为Object[]数组传递给 replaceMethod
        int argLen = method.Parameters.Count;
        if (!method.IsStatic)
        {
            argLen += 1;    //成员方法, 额外传参this
        }
        //创建数组对象
        ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Ldc_I4, argLen));
        ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Newarr, ObjectType));
        for (int i = 0; i < argLen; i++)
        {
            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Dup));
            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Ldc_I4, i));
            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Ldarg, i));

            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Box, ObjectType));
            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        }
        ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Call, injMethod));      //调用方法

        //如果返回布尔值, 额外判断释放结束方法
        if (onLinkClicked.ReturnType == typeof(bool))
        {
            Instruction endLabel = ilProcessor.Create(OpCodes.Ret);
            ilProcessor.InsertBefore(firstPoint, ilProcessor.Create(OpCodes.Brtrue, endLabel));     //如果返回值为true, 跳转endLabel

            Instruction endPoint = method.Body.Instructions[method.Body.Instructions.Count - 1];
            ilProcessor.InsertAfter(endPoint, endLabel);
        }
    }

    static void _Inject_TestMethod(ModuleDefinition module, MethodDefinition[] methods)
    {
        MethodInfo onTest = typeof(UnityEditor.Console.OnClicked).GetMethod("OnTest", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (onTest == null)
            return;

        MethodReference injMethod = module.ImportReference(onTest);

        HashSet<string> filters = new HashSet<string>()
        {
            HyperLinkClickedMethod,
            "LogChanged",
            "DoLogChanged",
            "OnEnable",
            "SetFilter",
            "SetActiveEntry",
            "OnGUI",
            "LoadIcons",
            "UpdateListView",
            "HasFlag",
            "SetFlag",
            "HasMode",
            "SearchField",
            "GetIconForErrorMode",
            "GetStyleForErrorMode",
            "StacktraceWithHyperlinks",
            "GetStatusStyleForErrorMode",
        };
        foreach (var method in methods)
        {
            if (method.IsConstructor || method.IsGetter || method.IsSetter)
                continue;
            if (filters.Contains(method.Name))
                continue;

            _InjectMethod(module, method, injMethod);
        }
    }

    static void _InjectMethod(ModuleDefinition module, MethodDefinition original, MethodReference insert)
    {
        ILProcessor ilProcessor = original.Body.GetILProcessor();
        Instruction instruction = original.Body.Instructions[0];

        int argLen = original.Parameters.Count;
        if (!original.IsStatic) argLen += 1;    //成员方法, 额外传参this
        argLen += 1;   //额外写入MethodName

        TypeReference ObjectType = module.ImportReference(typeof(System.Object));

        ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Nop));
        //创建Object数组
        ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldc_I4, argLen));
        ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Newarr, ObjectType));
        for (int i = 0; i < argLen; i++)
        {
            ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Dup));
            ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldc_I4, i));
            if (i == 0)     //写入MethodName
            {
                ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldstr, original.Name));
            }
            else            //取出arg参数
            {
                ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldarg, i - 1));
            }
            ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Box, ObjectType));
            ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Stelem_Ref));
        }

        ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Call, insert));
    }
}
