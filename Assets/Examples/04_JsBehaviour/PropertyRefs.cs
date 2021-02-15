using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
#endif

public class PropertyRefs : MonoBehaviour
{
    private ResultPair[] _pairs = null;
    public ResultPair[] Pairs
    {
        get
        {
            if (_pairs == null)
                _pairs = GenPairs();
            return _pairs;
        }
    }

    void Awake()
    {
        this.enabled = false;
    }
    internal ResultPair[] GenPairs()
    {
        if (ObjectPairs == null)
            return null;
        var pairs = ((IEnumerable<IPair>)ObjectPairs)
            .Concat(StringPairs)
            .Concat(NumberPairs)
            .Concat(BooleanPairs)
            .Concat(ObjectArrayPairs)
            .Concat(GameObjectArrayPairs)
            .ToList();
        pairs.Sort((v1, v2) => v1.getIndex < v2.getIndex ? -1 : v1.getIndex > v2.getIndex ? 1 : 0);
        return pairs.Select(o => new ResultPair(o)).ToArray();
    }

    #region 序列化字段
    [SerializeField]
    private String[] StringPairs;
    [SerializeField]
    private Number[] NumberPairs;
    [SerializeField]
    private Boolean[] BooleanPairs;
    [SerializeField]
    private Object[] ObjectPairs;
    [SerializeField]
    private ObjectArray[] ObjectArrayPairs;
    [SerializeField]
    private GameObjectArray[] GameObjectArrayPairs;
    #endregion

    public class ResultPair
    {
        public string key;
        public object value;
        internal ResultPair(IPair pair)
        {
            this.key = pair.getKey;
            this.value = pair.getValue;
        }
    }
    internal interface IPair
    {
        int getIndex { get; }
        string getKey { get; }
        object getValue { get; }
    }

    #region 序列化数据类
    internal class Pair<T> : IPair
    {
        public int index;
        public string key;
        public T value;

        public int getIndex { get { return this.index; } }
        public string getKey { get { return this.key; } }
        public object getValue { get { return this.value; } }
    }
    [System.Serializable]
    internal class String : Pair<System.String> { }
    [System.Serializable]
    internal class Number : Pair<System.Double> { }
    [System.Serializable]
    internal class Boolean : Pair<System.Boolean> { }
    [System.Serializable]
    internal class Object : Pair<UnityEngine.Object> { }
    [Options("Array/Object")]
    [System.Serializable]
    internal class ObjectArray : Pair<UnityEngine.Object[]> { }
    [Options("Array/GameObject")]
    [System.Serializable]
    internal class GameObjectArray : Pair<UnityEngine.GameObject[]> { }
    #endregion

    //Options 路径设置
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class OptionsAttribute : Attribute
    {
        internal string path { get; private set; }
        public OptionsAttribute(string path)
        {
            this.path = path;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PropertyRefs))]
[CanEditMultipleObjects]
public class PropertyRefs_CustomEditor : Editor
{
    //常量定义
    const float POPUP_WIDTH = 20f; //popup弹出菜单宽度
    const float FIELD_WIDTH = 120f; //value field width输入框宽度
    const string KEY_FIELD = "key"; //key field 名称
    const string VALUE_FIELD = "value"; //value field 名称
    const string INDEX_FIELD = "index"; //index field 名称

    private Object _scriptObject;
    //实例引用
    private PropertyRefs _instance;
    //字段映射
    private Dictionary<string, Type> _typeMapping;
    private Dictionary<string, string> _optionsMapping;
    private Dictionary<string, Action<Element>> _displayMapping;
    //Select Index
    private int _selectIndex;
    //fodout
    private static bool menuFoldout = true;
    private static bool arrayFoldout = true;
    //Check Switch
    private static bool checkKeyRedefinition = true;
    private static bool checkKeyValidity = true;

    void OnEnable()
    {
        //Debug.Log("OnEnable");
        _instance = target as PropertyRefs;
        _selectIndex = -1;
        _typeMapping = new Dictionary<string, Type>();
        _optionsMapping = new Dictionary<string, string>();
        _displayMapping = new Dictionary<string, Action<Element>>();
        LoadMapping();
        LoadScriptObject();
    }
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        _instance = null;
        _typeMapping.Clear();
        _optionsMapping.Clear();
        _displayMapping.Clear();
        _scriptObject = null;
    }
    public override void OnInspectorGUI()
    {
        if (_instance == null)
        {
            base.OnInspectorGUI();
            return;
        }
        UpdateProperties();

        var packages = GetPacksges();
        packages.Sort((v1, v2) =>
        {
            int index1 = v1.element.index, index2 = v2.element.index;
            return index1 < index2 ? -1 : index1 > index2 ? 1 : 0;
        });
        DisplayScriptName();
        DisplayScriptMenu();
        DisplayMenu(packages);
        if (arrayFoldout)
        {
            DisplayTitle();
            foreach (var package in packages)
            {
                package.Display();
            }
        }
        //应用更改
        ApplyModifiedProperties();
    }

    #region  
    void UpdateProperties()
    {
        serializedObject.Update();
    }
    void ApplyModifiedProperties()
    {
        serializedObject.ApplyModifiedProperties();
    }
    SerializedProperty GetSerializedProperty(string name)
    {
        return serializedObject.FindProperty(name);
    }
    //初始化Mapping字典(使用反射遍历)
    void LoadMapping()
    {
        var fields = typeof(PropertyRefs).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            Type elementType = field.FieldType.IsArray ? field.FieldType.GetElementType() : null;
            if (elementType != null
                && typeof(PropertyRefs.IPair).IsAssignableFrom(elementType)
                && GetSerializedProperty(field.Name) != null
                && CheckFields(elementType))
            {
                var display = elementType.GetField(VALUE_FIELD).FieldType.IsArray ? (Action<Element>)DisplayArray : (Action<Element>)DisplayItem;
                var options = elementType.GetCustomAttribute<PropertyRefs.OptionsAttribute>(false);
                var optionsName = options != null ? options.path : elementType.Name;
                _typeMapping.Add(field.Name, elementType);
                _optionsMapping.Add(field.Name, optionsName);
                _displayMapping.Add(field.Name, display);
            }
        }
    }
    //获得当前实例对应的脚本
    void LoadScriptObject()
    {
        var path = (from _p in AssetDatabase.GetAllAssetPaths()
                    where _p.EndsWith("PropertyRefs.cs")
                    select _p).FirstOrDefault();
        _scriptObject = path != null ? AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) : null;
    }
    //获得当前需要渲染的节点
    List<Package> GetPacksges()
    {
        var result = new List<Package>();
        foreach (var dictObj in _displayMapping)
        {
            var type = _typeMapping[dictObj.Key];
            var display = dictObj.Value;
            var arrayParentNode = GetSerializedProperty(dictObj.Key);
            for (int i = 0; i < arrayParentNode.arraySize; i++)
            {
                var node = arrayParentNode.GetArrayElementAtIndex(i);
                var element = new Element(node, type, i, arrayParentNode);
                result.Add(new Package(element, display));
            }
        }
        return result;
    }
    #endregion

    #region 渲染节点
    void DisplayScriptName()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Script", new GUIStyle("PR DisabledLabel"));
        EditorGUILayout.ObjectField(_scriptObject, typeof(void), false);
        EditorGUILayout.EndHorizontal();
    }
    void DisplayScriptMenu()
    {
        menuFoldout = EditorGUILayout.Foldout(menuFoldout, "MENU");
        if (menuFoldout)
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5f);
            if (GUILayout.Button("copy declare code [public]"))
            {
                var code = GenCode(_instance.GenPairs(), "public ", false);
                var editor = new TextEditor();
                editor.text = code;
                editor.OnFocus();
                editor.Copy();
                Debug.Log("已复制到剪贴板:\n" + code);
            }
            if (GUILayout.Button("copy declare code [private]"))
            {
                var code = GenCode(_instance.GenPairs(), "private ", false);
                var editor = new TextEditor();
                editor.text = code;
                editor.OnFocus();
                editor.Copy();
                Debug.Log("已复制到剪贴板:\n" + code);
            }
            checkKeyRedefinition = EditorGUILayout.Toggle("Check Key Redefinition", checkKeyRedefinition);
            checkKeyValidity = EditorGUILayout.Toggle("Check Key Validity", checkKeyValidity);
            //Check key redefinition
            PropertyRefs.ResultPair[] pairs;
            if (checkKeyRedefinition && (pairs = _instance.GenPairs()) != null)
            {
                var keys = new List<string>();
                for (int i = 0; i < pairs.Length; i++)
                {
                    var key = pairs[i].key;
                    if (keys.Contains(key))
                    {
                        EditorGUILayout.BeginHorizontal();
                        DisplayWarning(string.Format("key redefinition: \"{0}\" at {1}", key, i));
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                        keys.Add(key);
                }
            }
            GUILayout.Space(5f);
            EditorGUILayout.EndVertical();
        }
    }
    void DisplayMenu(List<Package> packages)
    {
        var count = packages.Count;
        EditorGUILayout.BeginHorizontal();
        arrayFoldout = EditorGUILayout.Foldout(arrayFoldout, "COUNT: " + count);
        GUILayout.FlexibleSpace();
        //Add 
        if (GUILayout.Button("+"))
        {
            PopupCreate(count);
        }
        //Remove 
        if (GUILayout.Button("-") && count > 0)
        {
            if (_selectIndex < 0 || _selectIndex >= count)
            {
                packages[count - 1].element.DeleteCommand();
                packages.RemoveAt(count - 1);
            }
            else
            {
                for (int i = _selectIndex + 1; i < packages.Count; i++)
                {
                    var element = packages[i].element;
                    element.index--;
                }
                packages[_selectIndex].element.DeleteCommand();
                packages.RemoveAt(_selectIndex);
                _selectIndex--;
            }
        }
        GUILayout.Space(5f);
        //Move Up 
        if (GUILayout.Button("↑") && _selectIndex > 0 && _selectIndex < count)
        {
            var element = packages[_selectIndex].element;
            element.index--;
            element = packages[_selectIndex - 1].element;
            element.index++;
            _selectIndex--;
        }
        //Move Down
        if (GUILayout.Button("↓") && _selectIndex >= 0 && _selectIndex < count - 1)
        {
            var element = packages[_selectIndex].element;
            element.index++;
            element = packages[_selectIndex + 1].element;
            element.index--;
            _selectIndex++;
        }
        EditorGUILayout.EndHorizontal();
    }
    void DisplayTitle()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("keys");
        EditorGUILayout.LabelField("values", GUILayout.Width(FIELD_WIDTH));
        GUILayout.Space(40f);
        EditorGUILayout.EndHorizontal();
    }
    void DisplayItem(Element element)
    {
        //EditorGUILayout.BeginVertical(_selectIndex == element.index ? "U2D.createRect" : "Box");
        EditorGUILayout.BeginVertical(_selectIndex == element.index ? "SelectionRect" : "Box");
        //Content
        EditorGUILayout.BeginHorizontal();
        DisplaySelect(element);
        element.key = EditorGUILayout.TextField(element.key, GUILayout.MinWidth(50f));
        DisplayField(element.valueNode, element.valueType);
        GUILayout.Space(5f);
        if (element.valueNode.propertyType == SerializedPropertyType.ObjectReference)
            DisplayComponentsAndTypes(element, element.valueNode.objectReferenceValue);
        else
            DisplayTypes(element);
        EditorGUILayout.EndHorizontal();
        //Check Name
        if (checkKeyValidity && !CheckKeyValidity(element.key))
        {
            EditorGUILayout.BeginHorizontal();
            DisplayWarning("Invalid naming");
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }
    void DisplayArray(Element element)
    {
        //Array
        var arrayParentNode = element.valueNode;

        EditorGUILayout.BeginVertical(_selectIndex == element.index ? "SelectionRect" : "Box");
        //Content
        EditorGUILayout.BeginHorizontal();
        DisplaySelect(element);
        element.key = EditorGUILayout.TextField(element.key, GUILayout.MinWidth(50f));
        var title = (arrayParentNode.isExpanded ? "▼ " : "► ") + element.valueType.Name;
        if (title.EndsWith("[]")) title = title.Replace("[]", "[" + arrayParentNode.arraySize + "]");
        if (GUILayout.Button(title, "AM HeaderStyle", GUILayout.Width(FIELD_WIDTH)))
        {
            arrayParentNode.isExpanded = !arrayParentNode.isExpanded;
        }
        GUILayout.Space(5f);
        DisplayTypes(element);
        EditorGUILayout.EndHorizontal();
        //Check Name
        if (checkKeyValidity && !CheckKeyValidity(element.key))
        {
            EditorGUILayout.BeginHorizontal();
            DisplayWarning("Invalid naming");
            EditorGUILayout.EndHorizontal();
        }
        if (arrayParentNode.isExpanded)
        {
            //Array Menu
            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            EditorGUILayout.LabelField("size:", GUILayout.Width(40f));
            arrayParentNode.arraySize = EditorGUILayout.IntField(arrayParentNode.arraySize);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("", "OL Plus"))
            {
                arrayParentNode.arraySize++;
            }
            if (GUILayout.Button("", "OL Minus") && arrayParentNode.arraySize > 0)
            {
                arrayParentNode.arraySize--;
            }
            EditorGUILayout.EndHorizontal();
            //Array Expanded
            if (arrayParentNode.arraySize > 0)
            {
                GUILayout.Space(2f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20f);
                EditorGUILayout.BeginVertical("HelpBox");
                for (int i = 0; i < arrayParentNode.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //Array Child Content
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(40f));
                    GUILayout.FlexibleSpace();
                    var node = arrayParentNode.GetArrayElementAtIndex(i);
                    DisplayField(node, element.valueType.GetElementType());
                    GUILayout.Space(5f);
                    if (node.propertyType == SerializedPropertyType.ObjectReference)
                        DisplayComponents(node, node.objectReferenceValue);
                    else
                        GUILayout.Space(POPUP_WIDTH);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(2f);
                }
                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();
    }

    void DisplayField(SerializedProperty node, Type selectedType)
    {
        var width = GUILayout.Width(FIELD_WIDTH);
        switch (node.propertyType)
        {
            case SerializedPropertyType.Integer:
                node.intValue = EditorGUILayout.IntField(node.intValue, width);
                break;
            case SerializedPropertyType.Boolean:
                node.boolValue = EditorGUILayout.Toggle(node.boolValue, width);
                break;
            case SerializedPropertyType.Float:
                node.doubleValue = EditorGUILayout.DoubleField(node.doubleValue, width);
                break;
            case SerializedPropertyType.String:
                node.stringValue = EditorGUILayout.TextField(node.stringValue, width);
                break;
            case SerializedPropertyType.Color:
                node.colorValue = EditorGUILayout.ColorField(node.colorValue, width);
                break;
            case SerializedPropertyType.ObjectReference:
                node.objectReferenceValue = EditorGUILayout.ObjectField(node.objectReferenceValue, selectedType, true, width);
                break;
            case SerializedPropertyType.Vector2:
                node.vector2Value = EditorGUILayout.Vector2Field("", node.vector2Value, width);
                break;
            case SerializedPropertyType.Vector3:
                node.vector3Value = EditorGUILayout.Vector3Field("", node.vector3Value, width);
                break;
            case SerializedPropertyType.Vector4:
                node.vector4Value = EditorGUILayout.Vector4Field("", node.vector4Value, width);
                break;
            case SerializedPropertyType.Rect:
                node.rectValue = EditorGUILayout.RectField(node.rectValue, width);
                break;
            case SerializedPropertyType.Bounds:
                node.boundsValue = EditorGUILayout.BoundsField(node.boundsValue, width);
                break;
            case SerializedPropertyType.Vector2Int:
                node.vector2IntValue = EditorGUILayout.Vector2IntField("", node.vector2IntValue, width);
                break;
            case SerializedPropertyType.Vector3Int:
                node.vector3IntValue = EditorGUILayout.Vector3IntField("", node.vector3IntValue, width);
                break;
            case SerializedPropertyType.RectInt:
                node.rectIntValue = EditorGUILayout.RectIntField(node.rectIntValue, width);
                break;
            case SerializedPropertyType.BoundsInt:
                node.boundsIntValue = EditorGUILayout.BoundsIntField(node.boundsIntValue, width);
                break;

            default:
                GUILayout.Space(FIELD_WIDTH);
                break;
        }
    }
    void DisplaySelect(Element element)
    {
        //if (GUILayout.Button("", "PaneOptions", GUILayout.Width(15f)))
        if (GUILayout.Button("", "PreSliderThumb", GUILayout.Width(15f)))
        {
            _selectIndex = _selectIndex != element.index ? element.index : -1;
        }
    }
    void DisplayTypes(Element element)
    {
        if (GUILayout.Button("", "AssetLabel Icon", GUILayout.Width(POPUP_WIDTH)))
        {
            PopupTypes(element);
        }
    }
    void DisplayComponentsAndTypes(Element element, Object obj)
    {
        if (GUILayout.Button("", "AssetLabel Icon", GUILayout.Width(POPUP_WIDTH)))
        {
            PopupComponentsAndTypes(element, obj);
        }
    }
    void DisplayComponents(SerializedProperty node, Object obj)
    {
        //TextFieldDropDown
        //AssetLabel Icon
        //PaneOptions
        if (GUILayout.Button("", "PaneOptions", GUILayout.Width(POPUP_WIDTH)))
        {
            PopupComponents(node, obj);
        }
    }
    void DisplayWarning(string content)
    {
        EditorGUILayout.LabelField("", new GUIStyle("CN EntryWarnIconSmall"), GUILayout.Width(20f));
        EditorGUILayout.LabelField(content);
    }
    #endregion

    #region 渲染弹出式菜单
    void PopupCreate(int index)
    {
        //var options = (from type in _typeMapping.Values select type.Name).ToArray();
        var options = _optionsMapping.Values.ToArray();
        CustomMenu(options, null, null, null, (value) =>
        {
            //Create Element
            var arrayParentNode = GetSerializedProperty(_typeMapping.Keys.ElementAt(value));
            arrayParentNode.arraySize++;
            var create = new Element(arrayParentNode.GetArrayElementAtIndex(arrayParentNode.arraySize - 1), _typeMapping.Values.ElementAt(value));
            create.index = index;
            create.key = KEY_FIELD + index;
            if (create.valueNode.isArray)
            {
                create.valueNode.arraySize = 0;
                create.valueNode.isExpanded = true;
            }
            create.Clean();
            //应用更改
            ApplyModifiedProperties();
        });
    }
    void PopupTypes(Element element)
    {
        var options = _optionsMapping.Values.ToArray();
        var selected = new[] { options.ToList().IndexOf(element.type.Name) };
        CustomMenu(options, null, selected, selected, (value) =>
        {
            //Create Element
            var arrayParentNode = GetSerializedProperty(_typeMapping.Keys.ElementAt(value));
            arrayParentNode.arraySize++;
            var create = new Element(arrayParentNode.GetArrayElementAtIndex(arrayParentNode.arraySize - 1), _typeMapping.Values.ElementAt(value));
            create.index = element.index;
            create.key = element.key;
            if (create.valueNode.isArray)
            {
                create.valueNode.arraySize = element.valueNode.isArray ? element.valueNode.arraySize : 0;
                create.valueNode.isExpanded = element.valueNode.isArray ? element.valueNode.isExpanded : true;
            }
            create.Clean();
            CopyObject(element, create);
            CopyArray(element, create);
            //Delete Element
            element.DeleteCommand();
            //应用更改
            ApplyModifiedProperties();
        });
    }
    void PopupComponentsAndTypes(Element element, Object obj)
    {
        var options = _optionsMapping.Values.ToArray();
        var selected = new[] { options.ToList().IndexOf(element.type.Name) };
        string[] separator = null;
        var objects = GetCompoents(obj);
        if (objects != null)
        {
            //Options
            var _options = new List<string>() { "<NULL>" };
            _options.AddRange(CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
            _options.AddRange(options);
            options = _options.ToArray();
            //Objects
            var _objects = objects.ToList();
            _objects.Insert(0, null);
            objects = _objects.ToArray();
            //Separator
            string nullable = null;
            separator = new List<string>(from _ in _objects select nullable) { }.ToArray();
            separator[0] = separator[separator.Length - 1] = "";
            //Select
            selected = new List<int>() { _objects.IndexOf(obj), selected[0] + objects.Length }.ToArray();
        }
        CustomMenu(options, separator, selected, selected, (value) =>
        {
            if (objects == null || value >= objects.Length)
            {
                value = value - (objects != null ? objects.Length : 0);
                //Create Element
                var arrayParentNode = GetSerializedProperty(_typeMapping.Keys.ElementAt(value));
                arrayParentNode.arraySize++;
                var create = new Element(arrayParentNode.GetArrayElementAtIndex(arrayParentNode.arraySize - 1), _typeMapping.Values.ElementAt(value));
                create.index = element.index;
                create.key = element.key;
                if (create.valueNode.isArray)
                {
                    create.valueNode.arraySize = element.valueNode.isArray ? element.valueNode.arraySize : 0;
                    create.valueNode.isExpanded = element.valueNode.isArray ? element.valueNode.isExpanded : true;
                }
                create.Clean();
                CopyObject(element, create);
                CopyArray(element, create);
                //Delete Element
                element.DeleteCommand();
            }
            else
            {
                element.valueNode.objectReferenceValue = objects[value];
            }
            //应用更改
            ApplyModifiedProperties();
        });
    }
    void PopupComponents(SerializedProperty node, Object obj)
    {
        var options = new[] { "<NULL>" };
        var selected = new[] { 0 };
        string[] separator = null;
        var objects = GetCompoents(obj);
        if (objects != null)
        {
            //Options
            var _options = new List<string>() { "<NULL>" };
            _options.AddRange(CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
            options = _options.ToArray();
            //Objects
            var _objects = objects.ToList();
            _objects.Insert(0, null);
            objects = _objects.ToArray();
            //Separator
            separator = new[] { "" };
            //Select
            selected = new List<int>() { _objects.IndexOf(obj) }.ToArray();
        }
        CustomMenu(options, separator, selected, selected, (value) =>
        {
            if (objects != null && value < objects.Length)
            {
                node.objectReferenceValue = objects[value];
                //应用更改
                ApplyModifiedProperties();
            }
        });
    }
    #endregion

    //在鼠标位置弹出菜单 (在GUI之后弹出面板)
    static void CustomMenu(string[] options, string[] separator, int[] selected, int[] disabled, Action<int> callback)
    {
        //EditorUtility.DisplayCustomMenu();  // 在指定区域弹出菜单 (需GUI方法内部调用)
        var _separator = new List<string>(separator != null ? separator : new string[0]);
        var _selected = new List<int>(selected != null ? selected : new int[0]);
        var _disabled = new List<int>(disabled != null ? disabled : new int[0]);
        var menu = new UnityEditor.GenericMenu();
        for (int i = 0; i < options.Length; i++)
        {
            var index = i;
            if (_disabled.Contains(index))
                menu.AddDisabledItem(new GUIContent(options[i]), _selected.Contains(index));
            else
                menu.AddItem(new GUIContent(options[i]), _selected.Contains(index), () => callback(index));
            if (i < _separator.Count && _separator[i] != null)
                menu.AddSeparator(_separator[i]);
        }
        menu.ShowAsContext();
    }
    /// <summary> 检测命名是否符合规则 </summary>
    static bool CheckKeyValidity(string name)
    {
        //#:    35
        //$:    36
        //0-9:  48-57
        //A-Z:  65-90
        //_:    95
        //a-z:  97-122
        if (string.IsNullOrEmpty(name))
            return false;
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (!(
                c == 35 && i == 0 ||
                c == 36 ||
                c >= 48 && c <= 57 && i != 0 ||
                c >= 65 && c <= 90 ||
                c == 95 ||
                c >= 97 && c <= 122))
                return false;
        }
        return true;
    }
    /// <summary> 检测字段是否符合类型需求 </summary>
    static bool CheckFields(Type type)
    {
        FieldInfo index, key;
        return typeof(PropertyRefs.IPair).IsAssignableFrom(type)
            && (index = type.GetField(INDEX_FIELD)) != null && index.FieldType == typeof(int)
            && (key = type.GetField(KEY_FIELD)) != null && key.FieldType == typeof(string)
            && type.GetField(VALUE_FIELD) != null;
    }
    /// <summary> 尝试Copy值 </summary>
    static void CopyObject(Element from, Element to)
    {
        if (from.valueNode.propertyType == SerializedPropertyType.ObjectReference
            && to.valueNode.propertyType == SerializedPropertyType.ObjectReference
            && from.valueNode.objectReferenceValue != null
            && to.valueType.IsAssignableFrom(from.valueNode.objectReferenceValue.GetType()))
        {
            to.valueNode.objectReferenceValue = from.valueNode.objectReferenceValue;
        }
    }
    static void CopyArray(Element from, Element to)
    {
        if (from.valueType.IsArray
            && to.valueType.IsArray
            && typeof(UnityEngine.Object).IsAssignableFrom(from.valueType.GetElementType())
            && typeof(UnityEngine.Object).IsAssignableFrom(to.valueType.GetElementType()))
        {
            var targetType = to.valueType.GetElementType();
            for (int i = 0; i < from.valueNode.arraySize; i++)
            {
                var node = from.valueNode.GetArrayElementAtIndex(i);
                if (node.objectReferenceValue != null
                && targetType.IsAssignableFrom(node.objectReferenceValue.GetType()))
                {
                    from.valueNode.GetArrayElementAtIndex(i).objectReferenceValue = node.objectReferenceValue;
                }
            }
        }
    }
    /// <summary> 检测Menu.Options是否有重复名称, 并返回更正后的Options </summary>
    static string[] CheckOptions(string[] options)
    {
        //重命名(重复名称-相同类型)
        var countDict = new Dictionary<string, int>();
        foreach (var option in options)
        {
            if (countDict.ContainsKey(option))
            {
                var count = countDict[option];
                countDict[option] = count == 0 ? 2 : ++count;
            }
            else countDict.Add(option, 0);
        }
        for (int i = options.Length - 1; i >= 0; i--)
        {
            var count = countDict[options[i]];
            if (count > 0)
            {
                countDict[options[i]] = count - 1;
                options[i] += "(" + count + ")";
            }
        }
        return options;
    }
    static Object[] GetCompoents(Object obj)
    {
        if (obj != null)
        {
            var result = new List<Object>();
            var type = obj.GetType();
            //获取GameObject和Transform组件
            var getObj = type.GetProperty("gameObject");
            var getTrf = type.GetProperty("transform");
            var gameObject = (getObj != null ? getObj.GetValue(obj) : null) as GameObject;
            var transform = (getTrf != null ? getTrf.GetValue(obj) : null) as Transform;
            //调用GetComponents方法获取所有组件, 如果有gameObject则从Gameobject对象中获取所有组件(排除obj自身对排序的干扰)
            if (gameObject != null)
                type = gameObject.GetType();
            var getComponents = (from method in type.GetMethods()
                                 let parames = method.GetParameters()
                                 where method.Name == "GetComponents"
                                    && method.ReturnType == typeof(Component[])
                                    && parames.Length == 1
                                    && parames[0].ParameterType == typeof(System.Type)
                                 select method).FirstOrDefault();
            if (getComponents != null)
            {
                var components = getComponents.Invoke(gameObject != null ? gameObject : obj, new object[] { typeof(Component) }) as Component[];
                foreach (var o in components)
                {
                    if (!result.Contains(o))
                        result.Add(o);
                }
            }
            //obj自身
            if (!result.Contains(obj))
                result.Add(obj);
            //通过Type名进行排序
            result = (from o in result orderby o.GetType().Name select o).ToList();
            //GameObject / Transform
            if (transform != null)
            {
                result.Remove(transform);
                result.Insert(0, transform);
            }
            if (gameObject != null)
            {
                result.Remove(gameObject);
                result.Insert(0, gameObject);
            }
            return result.ToArray();
        }
        return null;
    }
    static string GenCode(PropertyRefs.ResultPair[] pairs, string declareType, bool useFullname)
    {
        var resultCode = "";
        foreach (var pair in pairs)
        {
            if (!string.IsNullOrEmpty(resultCode))
                resultCode += "\n";
            var type = pair.value != null ? pair.value.GetType() : null;
            var typeStr = GetTypeName(type, useFullname);
            resultCode += declareType + pair.key + ": " + typeStr + ";";
        }
        return resultCode;
    }
    static string GetTypeName(Type type, bool useFullname)
    {
        if (type == null)
            return "undefined";
        //Array Type
        if (type.IsArray)
            return "System.Array$1<" + GetTypeName(type.GetElementType(), useFullname) + ">";
        //Value Mapping
        if (type.Equals(typeof(double)) || type.Equals(typeof(float)) || type.Equals(typeof(int)) || type.Equals(typeof(long)))
            return "number";
        if (type.Equals(typeof(long)))
            return "bigint";
        if (type.Equals(typeof(string)) || type.Equals(typeof(char)))
            return "string";
        if (type.Equals(typeof(bool)))
            return "boolean";
        if (type.Equals(typeof(DateTime)))
            return "Date";
        return useFullname ? type.FullName.Replace("+", ".") : type.Name;
    }
    struct Package
    {
        public Element element { get; private set; }
        public Action<Element> display { get; private set; }
        public Package(Element element, Action<Element> display)
        {
            this.element = element;
            this.display = display;
        }
        public void Display()
        {
            this.display(this.element);
        }
    }
    struct Element
    {
        /// <summary> node节点对应的Pair类型  </summary>
        public Type type { get; private set; }
        /// <summary> valueNode节点对应的类型(any)  </summary>
        public Type valueType { get; private set; }
        private int _arrayIndex;
        private SerializedProperty _node;
        private SerializedProperty _keyNode;
        private SerializedProperty _indexNode;
        private SerializedProperty _valueNode;
        private SerializedProperty _arrayParentNode;
        public SerializedProperty keyNode
        {
            get
            {
                if (_keyNode == null) _keyNode = _node.FindPropertyRelative(KEY_FIELD);
                return _keyNode;
            }
        }
        public SerializedProperty indexNode
        {
            get
            {
                if (_indexNode == null) _indexNode = _node.FindPropertyRelative(INDEX_FIELD);
                return _indexNode;
            }
        }
        public SerializedProperty valueNode
        {
            get
            {
                if (_valueNode == null) _valueNode = _node.FindPropertyRelative(VALUE_FIELD);
                return _valueNode;
            }
        }
        public string key
        {
            get { return this.keyNode.stringValue; }
            set { this.keyNode.stringValue = value; }
        }
        public int index
        {
            get { return this.indexNode.intValue; }
            set { this.indexNode.intValue = value; }
        }
        public Element(SerializedProperty node, Type pairType) : this(node, pairType, 0, null)
        {
        }
        public Element(SerializedProperty node, Type pairType, int arrayIndex, SerializedProperty arrayParentNode)
        {
            this._node = node;
            this.type = pairType;
            this.valueType = pairType.GetField(VALUE_FIELD).FieldType;
            this._arrayIndex = arrayIndex;
            this._arrayParentNode = arrayParentNode;
            this._keyNode = null;
            this._valueNode = null;
            this._indexNode = null;
        }
        /// <summary>从父节点上删除此节点</summary>
        public void DeleteCommand()
        {
            _arrayParentNode.DeleteArrayElementAtIndex(_arrayIndex);
        }
        /// <summary>清理"值"节点属性</summary>
        public void Clean()
        {
            if (valueNode.isArray)
            {
                for (int i = 0; i < valueNode.arraySize; i++)
                {
                    CleanNode(valueNode.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                CleanNode(valueNode);
            }

        }
        static void CleanNode(SerializedProperty node)
        {
            //期望的字段名称
            var expectedPropName = (GetTypeName(node.propertyType) + "Value").ToLower();
            var prop = (from _p in typeof(SerializedProperty).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        where _p.CanRead && _p.CanWrite && _p.Name.ToLower().Equals(expectedPropName)
                        select _p).FirstOrDefault();
            if (prop != null)
                prop.SetMethod.Invoke(node, new object[] { default });
            else
                throw new KeyNotFoundException(string.Format("expected prop name = {0}, but not found.", expectedPropName));
        }
        static string GetTypeName(SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Boolean:
                    return "bool";
            }
            return Enum.GetName(typeof(SerializedPropertyType), type);
        }
    }
}
#endif