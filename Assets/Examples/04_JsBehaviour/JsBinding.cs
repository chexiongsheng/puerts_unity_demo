#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Type = JsBinding.Type;

/// <summary>
/// 绑定参数
/// !!!!!!! 与之前的版本不兼容, 升级请谨慎选择!!!!!!!!
/// !!!!!!! 与之前的版本不兼容, 升级请谨慎选择!!!!!!!!
/// !!!!!!! 与之前的版本不兼容, 升级请谨慎选择!!!!!!!!
/// !!!!!!! 升级一时爽, 代码火葬场-----------!!!!!!!!
/// </summary>
public class JsBinding : JsComponent
{
    public Arg[] Args;

    [Serializable]
    public struct Arg
    {
        public string Name;
        public Type Type;
        public Object Object;
        public Object[] Array;
        public double Number;
        public string String;
    }
    public enum Type
    {
        Object,
        Array,
        Number,
        String,
    }

#if UNITY_EDITOR
    string Code(string declare = "")
    {
        var str = "";
        foreach (var arg in Args)
        {
            var type = "undefined";
            switch (arg.Type)
            {
                case Type.Number:
                    type = "number";
                    break;
                case Type.String:
                    type = "string";
                    break;
                case Type.Object:
                    if (arg.Object != null)
                        type = arg.Object.GetType().FullName.Replace("+", ".");
                    break;
                case Type.Array:
                    if (arg.Array.Length > 0 && arg.Array[0] != null)
                        type = "System.Array$1<" + arg.Array[0].GetType().FullName.Replace("+", ".") + ">";
                    else
                        type = "System.Array$1<undefined>";
                    break;
            }
            if (!string.IsNullOrEmpty(str)) str += "\n";
            str += declare + arg.Name + ": " + type + ";";
        }
        return str;
    }
    string CodeShort(string declare = "")
    {
        var str = "";
        foreach (var arg in Args)
        {
            var type = "undefined";
            switch (arg.Type)
            {
                case Type.Number:
                    type = "number";
                    break;
                case Type.String:
                    type = "string";
                    break;
                case Type.Object:
                    if (arg.Object != null)
                        type = arg.Object.GetType().Name.Replace("+", ".");
                    break;
                case Type.Array:
                    if (arg.Array.Length > 0 && arg.Array[0] != null)
                        type = "Array$1<" + arg.Array[0].GetType().Name.Replace("+", ".") + ">";
                    else
                        type = "Array$1<undefined>";
                    break;
            }
            if (!string.IsNullOrEmpty(str)) str += "\n";
            str += declare + arg.Name + ": " + type + ";";
        }
        return str;
    }
    void Copy(string content)
    {
        var editor = new TextEditor();
        editor.text = content;
        editor.SelectAll();
        editor.Copy();
    }
    string Paste()
    {
        var editor = new TextEditor();
        editor.SelectAll();
        if (editor.Paste())
            return editor.text;
        return null;
    }
    /// <summary> 将变量复制为js代码 </summary>
    [ContextMenu("JsCode/public")]
    void BasePublic()
    {
        var str = Code("public ");
        Copy(str);
        Debug.Log("已复制到剪贴板:\n" + str);
    }
    /// <summary> 将变量复制为js代码 </summary>
    [ContextMenu("JsCode/private")]
    void BasePrivate()
    {
        var str = Code("private ");
        Copy(str);
        Debug.Log("已复制到剪贴板:\n" + str);
    }
    /// <summary> 将变量复制为js代码 </summary>
    [ContextMenu("JsCode/public[short]")]
    void PublicShort()
    {
        var str = CodeShort("public ");
        Copy(str);
        Debug.Log("已复制到剪贴板:\n" + str);
    }
    /// <summary> 将变量复制为js代码 </summary>
    [ContextMenu("JsCode/private[short]")]
    void PrivateShort()
    {
        var str = CodeShort("private ");
        Copy(str);
        Debug.Log("已复制到剪贴板:\n" + str);
    }
    /// <summary>
    /// 将复制的Js代码的参数名追加到参数列表
    /// </summary>
    [ContextMenu("JsCode/append names")]
    void AppendNames()
    {
        var content = Paste();
        if (!string.IsNullOrEmpty(content))
        {
            var append = new List<Arg>(Args);
            content = content.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            Debug.Log(content);
            foreach (var line in content.Split(';'))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var arr = line.Split(':');
                var name = arr[0];
                var type = arr.Length > 1 ? arr[1] : "";

                name = name.Replace("public ", "").Replace("private ", "").Replace("protected ", "")
                    .Replace("set ", "").Replace("get ", "").Replace("(", "").Replace(")", "")
                    .Trim();

                append.Add(new Arg()
                {
                    Name = name,
                    Type =
                        type == "number" ? Type.Number :
                        type == "string" ? Type.String :
                        type.IndexOf("Array$1<") >= 0 ? Type.Array :
                        Type.Object
                });
            }

            Args = append.ToArray();
        }
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(JsBinding))]
[CanEditMultipleObjects]
public class JsBindingEditor : Editor
{
    //public static JsBindingEditor Instance { get; private set; }
    private JsBinding instance;
    private SerializedProperty argsProp;

    //当前选中行
    private int select;
    //组件状态缓存
    private Dictionary<SerializedProperty, PropObjectState> propertys;

    void OnEnable()
    {
        instance = target as JsBinding;
        argsProp = serializedObject.FindProperty("Args");

        //Debug.Log("OnEnable");
        select = -1;
        if (propertys != null) propertys.Clear();
        propertys = new Dictionary<SerializedProperty, PropObjectState>();
    }
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        if (propertys != null) propertys.Clear();
        propertys = null;
    }

    public override void OnInspectorGUI()
    {
        if (instance == null || argsProp == null)
        {
            base.OnInspectorGUI();
            return;
        }
        serializedObject.Update();

        //Menu
        DisplayMenu();
        //Title
        DisplayTitle();
        //Element Array
        if (argsProp.isExpanded)
        {
            for (int i = 0; i < argsProp.arraySize; i++)
            {
                var prop = new PropElement(argsProp.GetArrayElementAtIndex(i));
                switch (prop.Type)
                {
                    case Type.Number:
                        DisplayNumber(i, prop);
                        break;
                    case Type.String:
                        DisplayString(i, prop);
                        break;
                    case Type.Object:
                        DisplayObject(i, prop);
                        break;
                    case Type.Array:
                        DisplayArray(i, prop);
                        break;
                }
            }
        }

        //保存更改
        serializedObject.ApplyModifiedProperties();
    }
    private void DisplayMenu()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Size:", GUILayout.Width(40f));
        argsProp.arraySize = EditorGUILayout.IntField(argsProp.arraySize, GUILayout.Width(50f));
        GUILayout.FlexibleSpace();
        //Refresh
        if (GUILayout.Button("○"))
        {
            if (propertys != null) propertys.Clear();
        }
        GUILayout.Space(5f);
        //Add Row
        if (GUILayout.Button("+"))
        {
            argsProp.arraySize++;
            select = argsProp.arraySize - 1;
            //Name
            argsProp.GetArrayElementAtIndex(select).FindPropertyRelative("Name").stringValue = "arg" + select;
        }
        //Remove Row
        if (GUILayout.Button("-"))
        {
            if (select >= 0)
            {
                argsProp.DeleteArrayElementAtIndex(select);
                if (select >= argsProp.arraySize)
                    select = argsProp.arraySize - 1;
            }
            else if (argsProp.arraySize > 0)
                argsProp.arraySize--;
        }
        GUILayout.Space(5f);
        //Move Up Row
        if (GUILayout.Button("↑") && select > 0)
        {
            argsProp.MoveArrayElement(select, --select);
        }
        //Move Down Row
        if (GUILayout.Button("↓") && select >= 0 && select < argsProp.arraySize - 1)
        {
            argsProp.MoveArrayElement(select, ++select);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayTitle()
    {
        EditorGUILayout.BeginHorizontal();
        argsProp.isExpanded = EditorGUILayout.Foldout(argsProp.isExpanded, "Name");
        GUILayout.Space(10f);
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Object", GUILayout.Width(100f));
        EditorGUILayout.LabelField("Type", GUILayout.Width(100f));
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayNumber(int index, PropElement prop)
    {
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        //Content
        prop.Name = GUILayout.TextField(prop.Name, GUILayout.MinWidth(50f));
        prop.Number = EditorGUILayout.DoubleField(prop.Number, GUILayout.Width(100f));
        prop.Type = (Type)EditorGUILayout.EnumPopup(prop.Type, GUILayout.Width(100f));
        //Select
        var tog = select == index;
        if (EditorGUILayout.Toggle(tog, GUILayout.Width(15f)))
            select = index;
        else if (tog)
            select = -1;

        EditorGUILayout.EndHorizontal();
    }
    private void DisplayString(int index, PropElement prop)
    {
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        //Content
        prop.Name = GUILayout.TextField(prop.Name, GUILayout.MinWidth(50f));
        prop.String = EditorGUILayout.TextField(prop.String, GUILayout.Width(100f));
        prop.Type = (Type)EditorGUILayout.EnumPopup(prop.Type, GUILayout.Width(100f));
        //Select
        var tog = select == index;
        if (EditorGUILayout.Toggle(tog, GUILayout.Width(15f)))
            select = index;
        else if (tog)
            select = -1;

        EditorGUILayout.EndHorizontal();
    }
    private void DisplayObject(int index, PropElement prop)
    {
        var state = GetObjectState(prop.elObject);
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        //Content
        prop.Name = GUILayout.TextField(prop.Name, GUILayout.MinWidth(50f));
        var obj = EditorGUILayout.ObjectField(prop.Object, typeof(Object), true, GUILayout.Width(100f));
        if (obj == null)
            prop.Type = (Type)EditorGUILayout.EnumPopup(prop.Type, GUILayout.Width(100f));
        else
        {
            var srcIndex = state.index;
            var endIndex = EditorGUILayout.Popup(srcIndex, state.names, GUILayout.Width(100f));
            //Update
            if (prop.Object != obj)
            {
                var before = state.Now();
                //New Object
                prop.Object = obj;
                state = GetObjectState(prop.elObject);
                //Index
                srcIndex = int.MinValue;
                endIndex = state.IndexOf(before);
            }
            if (srcIndex != endIndex)
            {
                state.index = endIndex;
                if (endIndex >= 0 && endIndex < state.components.Count)
                    prop.Object = state.components[endIndex];
            }
        }
        //Select
        var tog = select == index;
        if (EditorGUILayout.Toggle(tog, GUILayout.Width(15f)))
            select = index;
        else if (tog)
            select = -1;

        EditorGUILayout.EndHorizontal();
    }
    private void DisplayArray(int index, PropElement prop)
    {
        var array = prop.elArray;
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        //Content
        prop.Name = GUILayout.TextField(prop.Name, GUILayout.MinWidth(50f));
        GUILayout.Space(10f);
        array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, "<Array>");
        GUILayout.Space(40f);
        prop.Type = (Type)EditorGUILayout.EnumPopup(prop.Type, GUILayout.Width(100f));
        //Select
        var tog = select == index;
        if (EditorGUILayout.Toggle(tog, GUILayout.Width(15f)))
            select = index;
        else if (tog)
            select = -1;

        EditorGUILayout.EndHorizontal();
        //Expanded Array
        if (array.isExpanded)
        {
            DisplayArrayMenu(array);
            for (int i = 0; i < array.arraySize; i++)
            {
                DisplayArrayRow(i, array.GetArrayElementAtIndex(i));
            }
        }
    }
    private void DisplayArrayMenu(SerializedProperty array)
    {
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        EditorGUILayout.LabelField("Array:", GUILayout.Width(40f));
        array.arraySize = EditorGUILayout.IntField(array.arraySize, GUILayout.Width(50f));
        GUILayout.Space(10f);
        GUILayout.FlexibleSpace();
        //Add Row
        if (GUILayout.Button("+"))
        {
            array.arraySize++;
        }
        //Remove Row
        if (GUILayout.Button("-") && array.arraySize > 0)
        {
            array.arraySize--;
        }
        GUILayout.Space(15f);

        EditorGUILayout.EndHorizontal();
    }
    private void DisplayArrayRow(int index, SerializedProperty element)
    {
        var state = GetObjectState(element);
        //GUIStyle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        //Content
        EditorGUILayout.LabelField("Index " + index, GUILayout.MinWidth(40f));
        var obj = EditorGUILayout.ObjectField(element.objectReferenceValue, typeof(Object), true, GUILayout.Width(100f));
        var srcIndex = state.index;
        var endIndex = EditorGUILayout.Popup(srcIndex, state.names, GUILayout.Width(100f));
        //Update
        if (element.objectReferenceValue != obj)
        {
            var before = state.Now();
            //New Object
            element.objectReferenceValue = obj;
            state = GetObjectState(element);
            //Index
            srcIndex = int.MinValue;
            endIndex = state.IndexOf(before);
        }
        if (srcIndex != endIndex)
        {
            state.index = endIndex;
            if (endIndex >= 0 && endIndex < state.components.Count)
                element.objectReferenceValue = state.components[endIndex];
        }
        GUILayout.Space(15f);

        EditorGUILayout.EndHorizontal();
    }

    PropObjectState GetObjectState(SerializedProperty prop)
    {
        PropObjectState state;
        if (!this.propertys.TryGetValue(prop, out state) || state.Object != prop.objectReferenceValue)
        {
            //获取当前组件信息
            var components = GetCompoents(prop.objectReferenceValue);
            var index = components.IndexOf(prop.objectReferenceValue);
            var names = (from c in components where c != null select c.GetType().Name).ToArray();
            //重命名(重复名称-相同类型)
            var name_dict = new Dictionary<string, int>();
            foreach (var name in names)
            {
                if (name_dict.ContainsKey(name))
                {
                    var count = name_dict[name];
                    name_dict[name] = count == 0 ? 2 : ++count;
                }
                else name_dict.Add(name, 0);
            }
            for (int i = names.Length - 1; i >= 0; i--)
            {
                var count = name_dict[names[i]];
                if (count > 0)
                {
                    name_dict[names[i]] = count - 1;
                    names[i] += "(" + count + ")";
                }
            }
            //加入<NONE>选项
            components = new List<Object>(components) { null };
            names = new List<string>(names) { "<NONE>" }.ToArray();
            //创建State对象
            state = new PropObjectState(index, names, components);
            state.Object = prop.objectReferenceValue;

            this.propertys.Remove(prop);
            this.propertys.Add(prop, state);
        }
        return state;
    }
    static List<Object> GetCompoents(Object obj)
    {
        if (obj != null && !obj.GetType().IsValueType)
        {
            var result = new List<Object>();
            var type = obj.GetType();
            //获取GameObject和Transform组件
            PropertyInfo get_obj = type.GetProperty("gameObject"), get_trf = type.GetProperty("transform");
            var gobj = (get_obj != null ? get_obj.GetValue(obj) : null) as GameObject;
            var gtrf = (get_trf != null ? get_trf.GetValue(obj) : null) as Transform;
            //调用GetComponents方法获取所有组件, 如果有gameObject则从Gameobject对象中获取所有组件(排除obj自身对排序的干扰)
            if (gobj != null) type = gobj.GetType();
            MethodInfo get_components = (
                from method in type.GetMethods()
                where method.Name == "GetComponents"
                   && method.ReturnType == typeof(Component[])
                   && method.GetParameters().Length == 1
                   && method.GetParameters()[0].ParameterType == typeof(System.Type)
                select method).FirstOrDefault();
            if (get_components != null)
            {
                var components = get_components.Invoke(gobj != null ? gobj : obj, new object[] { typeof(Component) }) as Component[];
                foreach (var o in components)
                {
                    if (!result.Contains(o)) result.Add(o);
                }
            }
            //obj自身
            if (!result.Contains(obj)) result.Add(obj);
            //通过Type名进行排序
            result = (from o in result orderby o.GetType().Name select o).ToList();
            //GameObject / Transform
            if (gtrf != null) { result.Remove(gtrf); result.Insert(0, gtrf); }
            if (gobj != null) { result.Remove(gobj); result.Insert(0, gobj); }

            return result;
        }
        return new List<Object>() { };
    }
    private class PropElement
    {
        public SerializedProperty element { get; private set; }
        public SerializedProperty elName { get; private set; }
        public SerializedProperty elType { get; private set; }
        public SerializedProperty elObject { get; private set; }
        public SerializedProperty elArray { get; private set; }
        public SerializedProperty elNumber { get; private set; }
        public SerializedProperty elString { get; private set; }
        public string Name
        {
            get { return this.elName.stringValue; }
            set { this.elName.stringValue = value; }
        }
        public Type Type
        {
            get { return (Type)this.elType.intValue; }
            set { this.elType.intValue = (int)value; }
        }
        public Object Object
        {
            get { return this.elObject.objectReferenceValue; }
            set { this.elObject.objectReferenceValue = value; }
        }
        public double Number
        {
            get { return this.elNumber.doubleValue; }
            set { this.elNumber.doubleValue = value; }
        }
        public string String
        {
            get { return this.elString.stringValue; }
            set { this.elString.stringValue = value; }
        }
        public PropElement(SerializedProperty element)
        {
            this.element = element;
            this.elName = element.FindPropertyRelative("Name");
            this.elType = element.FindPropertyRelative("Type");
            this.elObject = element.FindPropertyRelative("Object");
            this.elArray = element.FindPropertyRelative("Array");
            this.elNumber = element.FindPropertyRelative("Number");
            this.elString = element.FindPropertyRelative("String");
        }
    }
    private class PropObjectState
    {
        public Object Object { get; set; }
        public int index { get; set; }
        public string[] names { get; }
        public List<Object> components { get; }
        public PropObjectState(int index, string[] names, List<Object> components)
        {
            this.index = index;
            this.names = names;
            this.components = components;
        }
        public string Now()
        {
            if (index >= 0 && index < names.Length)
                return names[index];
            return "";
        }
        public int IndexOf(string type)
        {
            //Type是重命名的类型
            var repeat_i = type.IndexOf("(");
            if (repeat_i >= 0)
                type = type.Substring(0, repeat_i);
            //在names中查找Type, Name中可能包含重命名
            for (int i = 0; i < names.Length; i++)
            {
                if (type == names[i] || names[i].Contains(type + "("))
                    return i;
            }
            return index;
        }
    }
}
//监听场景保存事件(手动存档?)
public class SaveScene : UnityEditor.AssetModificationProcessor
{
    static void OnWillSaveAssets(string[] names)
    {
        //Debug.Log("Save Prop1");
        //保存更改
        //serializedObject.ApplyModifiedProperties();
    }

    //快捷键绑定, 文档参考: https://docs.unity3d.com/ScriptReference/MenuItem.html
    //[MenuItem("File/Save %s")]
    static void Save()
    {
        Debug.Log("Save Prop2");

        //保存更改
        //serializedObject.ApplyModifiedProperties();
        //更新更改
        //serializedObject.Update();
    }
}
#endif