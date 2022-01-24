using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class JsPropertys : MonoBehaviour
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
        var pairsArray = new IEnumerable<IPair>[] {
            ObjectPairs,
            StringPairs,
            NumberPairs,
            BooleanPairs,
            ObjectArrayPairs,
            GameObjectArrayPairs
        };
        var list = (from pairs in pairsArray
                    where pairs != null
                    from pair in pairs
                    where pair != null
                    select pair).ToList();
        list.Sort((v1, v2) => v1.getIndex < v2.getIndex ? -1 : v1.getIndex > v2.getIndex ? 1 : 0);
        return list.Select(o => new ResultPair(o)).ToArray();
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
namespace UnityEditor.GUI
{
    using System.Reflection;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(JsPropertys))]
    //[CanEditMultipleObjects]
    public class JsPropertys_CustomEditor : Editor
    {
        private Object _scriptObject;
        //实例引用
        private JsPropertys _instance;
        public DrawerInfo drawInfo { get; private set; }
        private ReorderableList drawList;
        //fodout
        private static bool menuFoldout
        {
            get { return ValueGetter("menuFoldout", true); }
            set { ValueSetter("menuFoldout", value); }
        }
        //Check Switch
        private static bool checkKeyRedefinition
        {
            get { return ValueGetter("checkKeyRedefinition", true); }
            set { ValueSetter("checkKeyRedefinition", value); }
        }
        private static bool checkKeyValidity
        {
            get { return ValueGetter("checkKeyValidity", true); }
            set { ValueSetter("checkKeyValidity", value); }
        }

        void OnEnable()
        {
            //Debug.Log("OnEnable");
            _scriptObject = null;
            _instance = target as JsPropertys;
            if (_instance == null)
                return;

            //script object
            var targetName = $"/{nameof(JsPropertys)}.cs";
            var targetPath = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.EndsWith(targetName) && AssetDatabase.GetMainAssetTypeAtPath(p) == typeof(MonoScript));
            if (!string.IsNullOrEmpty(targetPath))
            {
                _scriptObject = AssetDatabase.LoadAssetAtPath(targetPath, typeof(MonoScript));
            }

            drawInfo = CreateDrawerInfo();

            var display = new Display(drawInfo);
            display.AddDrawer<StringDrawer>(typeof(JsPropertys.String));
            display.AddDrawer<NumberDrawer>(typeof(JsPropertys.Number));
            display.AddDrawer<BooleanDrawer>(typeof(JsPropertys.Boolean));
            display.AddDrawer<ObjectDrawer>(typeof(JsPropertys.Object));
            display.AddDrawer<ObjectArrayDrawer>(typeof(JsPropertys.ObjectArray));
            display.AddDrawer<GameObjectArrayDrawer>(typeof(JsPropertys.GameObjectArray));

            var elements = new List<Element>();
            drawList = new ReorderableList(elements, typeof(Element), true, true, true, true);
            drawList.elementHeightCallback = (index) =>
            {//元素盖度
                return display.GetHeight(elements[index]);
            };
            drawList.drawElementCallback = (rect, index, selected, focused) =>
            {//绘制元素
                display.Draw(rect, elements[index]);
            };
            drawList.drawHeaderCallback = (Rect rect) =>
            {//绘制表头
                UnityEngine.GUI.Label(rect, "Propertys");
            };
            drawList.onRemoveCallback = (ReorderableList list) =>
            {
                elements[list.index].DeleteCommand();
                drawInfo.ApplyModifiedProperties();
                elements.Clear();
                elements.AddRange(GetElements());
            };
            drawList.onAddCallback = (ReorderableList list) =>
            {
                DisplayUtility.PopupCreate(drawInfo, elements.Count, () =>
                {
                    drawInfo.ApplyModifiedProperties();
                });
            };
            drawList.onChangedCallback = (a) =>
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    var element = elements[i];
                    element.index = i;
                }
                drawInfo.ApplyModifiedProperties();
            };
        }
        void OnDisable()
        {
            //Debug.Log("OnDisable");
            _instance = null;
            drawInfo = null;
            drawList = null;
            _scriptObject = null;
        }
        public override void OnInspectorGUI()
        {
            if (_instance == null)
            {
                base.OnInspectorGUI();
                return;
            }
            drawInfo.UpdateProperties();

            DisplayScript();

            var elements = drawList.list as List<Element>;
            elements.Clear();
            elements.AddRange(GetElements());
            drawList.DoLayoutList();

            DisplayMenu();
            drawInfo.ApplyModifiedProperties();
        }

        #region  

        //获得当前需要渲染的节点
        public List<Element> GetElements()
        {
            var result = new List<Element>();
            foreach (var dictObj in drawInfo.typeMapping)
            {
                var type = dictObj.Value;
                var arrayParent = drawInfo.GetSerializedProperty(dictObj.Key);
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    var element = new Element(arrayParent.GetArrayElementAtIndex(i), type, i, arrayParent);
                    result.Add(element);
                }
            }
            result.Sort((v1, v2) => v1.index > v2.index ? 1 : v1.index < v2.index ? -1 : 0);
            return result;
        }
        private DrawerInfo CreateDrawerInfo()
        {
            DrawerInfo drawerInfo = null;
            if (_instance != null)
            {
                var typeMapping = new Dictionary<string, Type>();
                var optionsMapping = new Dictionary<string, string>();
                var fields = typeof(JsPropertys).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    Type elementType = field.FieldType.IsArray ? field.FieldType.GetElementType() : null;
                    if (elementType != null
                        && typeof(JsPropertys.IPair).IsAssignableFrom(elementType)
                        && serializedObject.FindProperty(field.Name) != null
                        && DisplayUtility.CheckFields(elementType))
                    {

                        var options = elementType.GetCustomAttribute<JsPropertys.OptionsAttribute>(false);
                        var optionsName = options != null ? options.path : elementType.Name;
                        typeMapping.Add(field.Name, elementType);
                        optionsMapping.Add(field.Name, optionsName);
                    }
                }
                drawerInfo = new DrawerInfo()
                {
                    serializedObject = serializedObject,
                    typeMapping = typeMapping,
                    optionsMapping = optionsMapping,
                };
            }
            return drawerInfo;
        }

        static Dictionary<string, bool> _cacheValues = new Dictionary<string, bool>();
        static bool ValueGetter(string key, bool defaultValue)
        {
            bool value;
            if (!_cacheValues.TryGetValue(key, out value))
            {
                value = EditorPrefs.GetBool(key, defaultValue);
                _cacheValues.Add(key, value);
            }
            return value;
        }
        static void ValueSetter(string key, bool value)
        {
            if (value != ValueGetter(key, !value))
            {
                EditorPrefs.SetBool(key, value);
                _cacheValues[key] = value;
            }
        }
        #endregion

        void DisplayScript()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Script", GUILayout.Width(100f));
                EditorGUILayout.ObjectField(_scriptObject, typeof(void), false);
                EditorGUILayout.EndHorizontal();
            }
        }
        void DisplayMenu()
        {
            menuFoldout = EditorGUILayout.Foldout(menuFoldout, "MENU");
            if (menuFoldout)
            {
                EditorGUILayout.BeginVertical("Box");
                GUILayout.Space(5f);
                if (GUILayout.Button("copy declare code [public]"))
                {
                    var code = DisplayUtility.GenCode(_instance.GenPairs(), "public", false);
                    var editor = new TextEditor();
                    editor.text = code;
                    editor.OnFocus();
                    editor.Copy();
                    Debug.Log("已复制到剪贴板:\n" + code);
                }
                if (GUILayout.Button("copy declare code [private]"))
                {
                    var code = DisplayUtility.GenCode(_instance.GenPairs(), "private", false);
                    var editor = new TextEditor();
                    editor.text = code;
                    editor.OnFocus();
                    editor.Copy();
                    Debug.Log("已复制到剪贴板:\n" + code);
                }
                if (GUILayout.Button("parse declare code[add names]"))
                {
                    var editor = new TextEditor();
                    editor.OnFocus();
                    editor.Paste();
                    var code = editor.text;
                    if (!string.IsNullOrEmpty(code))
                    {
                        Debug.Log("已获取以下内容:\n" + code);
                        DisplayUtility.PraseCode(this, code);
                    }
                }
                //Check key Toggles
                EditorGUILayout.BeginHorizontal();
                checkKeyRedefinition = EditorGUILayout.Toggle("", checkKeyRedefinition, GUILayout.Width(20f));
                EditorGUILayout.LabelField("check key redefinition");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                checkKeyValidity = EditorGUILayout.Toggle("", checkKeyValidity, GUILayout.Width(20f));
                EditorGUILayout.LabelField("check key validity");
                EditorGUILayout.EndHorizontal();
                //Check key redefinition
                JsPropertys.ResultPair[] pairs;
                if (checkKeyRedefinition && (pairs = _instance.GenPairs()) != null)
                {
                    var keys = new List<string>();
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        var key = pairs[i].key;
                        if (keys.Contains(key))
                        {
                            EditorGUILayout.BeginHorizontal();
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

        [InitializeOnLoadMethod]
        static void Init()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }
        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            //heldShift = e.shift;
            //heldControl = e.control || e.command;
        }
    }

    public class Element
    {
        /// <summary> node节点对应的Pair类型  </summary>
        public Type type { get; private set; }
        /// <summary> valueNode节点对应的类型(any)  </summary>
        public Type valueType { get; private set; }
        public int arrayIndex { get; private set; }
        public SerializedProperty arrayParent { get; private set; }
        private SerializedProperty _node;
        private SerializedProperty _keyNode;
        private SerializedProperty _indexNode;
        private SerializedProperty _valueNode;
        public SerializedProperty node { get => _node; }
        public SerializedProperty keyNode
        {
            get
            {
                if (_keyNode == null) _keyNode = _node.FindPropertyRelative("key");
                return _keyNode;
            }
        }
        public SerializedProperty indexNode
        {
            get
            {
                if (_indexNode == null) _indexNode = _node.FindPropertyRelative("index");
                return _indexNode;
            }
        }
        public SerializedProperty valueNode
        {
            get
            {
                if (_valueNode == null) _valueNode = _node.FindPropertyRelative("value");
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
        public Element(SerializedProperty node, Type pairType, int arrayIndex, SerializedProperty arrayParent)
        {
            this._node = node;
            this.type = pairType;
            this.valueType = pairType.GetField("value").FieldType;
            this.arrayIndex = arrayIndex;
            this.arrayParent = arrayParent;
            this._keyNode = null;
            this._valueNode = null;
            this._indexNode = null;
        }
        /// <summary>从父节点上删除此节点</summary>
        public void DeleteCommand()
        {
            arrayParent.DeleteArrayElementAtIndex(arrayIndex);
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
    public class DrawerInfo
    {
        public Element element { get; set; }
        public SerializedObject serializedObject { get; set; }
        public Dictionary<string, Type> typeMapping { get; set; }
        public Dictionary<string, string> optionsMapping { get; set; }

        public string GetOptionsName(Type type)
        {
            var key = typeMapping.Where(o => o.Value == type).FirstOrDefault().Key;
            if (!string.IsNullOrEmpty(key))
                return optionsMapping[key];
            return string.Empty;
        }
        public void UpdateProperties()
        {
            serializedObject.Update();
        }
        public void ApplyModifiedProperties()
        {
            serializedObject?.ApplyModifiedProperties();
        }
        public SerializedProperty GetSerializedProperty(string name)
        {
            return serializedObject?.FindProperty(name);
        }
    }
    public class Display
    {
        private DrawerInfo drawInfo;
        private Dictionary<Type, Drawer> drawerDict;

        public Display(DrawerInfo drawInfo)
        {
            this.drawInfo = drawInfo;
            this.drawerDict = new Dictionary<Type, Drawer>();
        }
        public void AddDrawer<T>(Type type) where T : Drawer, new()
        {
            var drawer = new T();
            drawer.drawInfo = drawInfo;
            drawerDict.Add(type, drawer);
        }
        public void Draw(Rect position, Element element)
        {
            if (drawerDict.TryGetValue(element.type, out var drawer))
            {
                drawer.element = element;
                drawer.Draw(position, GUIContent.none);
            }
        }

        public float GetHeight(Element element)
        {
            if (drawerDict.TryGetValue(element.type, out var drawer))
            {
                drawer.element = element;
                return drawer.GetHeight();
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }
    public class DisplayUtility
    {
        public static void PopupCreate(DrawerInfo info, int index, Action callback = null)
        {
            var options = info.optionsMapping.Values.ToArray();
            CustomMenu(options, null, null, null, (value) =>
            {
                //Create Element
                var arrayParent = info.GetSerializedProperty(info.typeMapping.Keys.ElementAt(value));
                arrayParent.arraySize++;
                var create = new Element(arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1)
                    , info.typeMapping.Values.ElementAt(value));
                create.index = index;
                create.key = "key" + index;
                if (create.valueNode.isArray)
                {
                    create.valueNode.arraySize = 0;
                    create.valueNode.isExpanded = true;
                }
                create.Clean();
                //应用更改
                info.ApplyModifiedProperties();

                callback?.Invoke();
            });
        }
        public static void PopupTypes(DrawerInfo info, Element element)
        {
            var options = info.optionsMapping.Values.ToArray();
            var selected = new[] { options.ToList().IndexOf(info.GetOptionsName(element.type)) };
            DisplayUtility.CustomMenu(options, null, selected, selected, (value) =>
            {
                //Create Element
                var arrayParent = info.GetSerializedProperty(info.typeMapping.Keys.ElementAt(value));
                arrayParent.arraySize++;
                var create = new Element(arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1)
                    , info.typeMapping.Values.ElementAt(value));
                create.index = element.index;
                create.key = element.key;
                if (create.valueNode.isArray)
                {
                    create.valueNode.arraySize = element.valueNode.isArray ? element.valueNode.arraySize : 0;
                    create.valueNode.isExpanded = element.valueNode.isArray ? element.valueNode.isExpanded : true;
                }
                create.Clean();
                DisplayUtility.Copy(element, create);
                //Delete Element
                element.DeleteCommand();
                //应用更改
                info.ApplyModifiedProperties();
            });
        }
        public static void PopupComponentsAndTypes(DrawerInfo info, Element element, Object obj, Type targetType)
        {
            var options = info.optionsMapping.Values.ToArray();
            var selected = new[] { options.ToList().IndexOf(info.GetOptionsName(element.type)) };
            string[] separator = null;
            var objects = DisplayUtility.GetCompoents(obj, targetType);
            if (objects != null)
            {
                //Options
                var _options = new List<string>() { "<NULL>" };
                _options.AddRange(DisplayUtility.CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
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
            DisplayUtility.CustomMenu(options, separator, selected, selected, (value) =>
            {
                if (objects == null || value >= objects.Length)
                {
                    value = value - (objects != null ? objects.Length : 0);
                    //Create Element
                    var arrayParent = info.GetSerializedProperty(info.typeMapping.Keys.ElementAt(value));
                    arrayParent.arraySize++;
                    var create = new Element(arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1)
                        , info.typeMapping.Values.ElementAt(value));
                    create.index = element.index;
                    create.key = element.key;
                    if (create.valueNode.isArray)
                    {
                        create.valueNode.arraySize = element.valueNode.isArray ? element.valueNode.arraySize : 0;
                        create.valueNode.isExpanded = element.valueNode.isArray ? element.valueNode.isExpanded : true;
                    }
                    create.Clean();
                    Copy(element, create);
                    //Delete Element
                    element.DeleteCommand();
                }
                else
                {
                    element.valueNode.objectReferenceValue = objects[value];
                }
                //应用更改
                info.ApplyModifiedProperties();
            });
        }
        public static void PopupComponents(DrawerInfo info, SerializedProperty node, Object obj, Type targetType)
        {
            var options = new[] { "<NULL>" };
            var selected = new[] { 0 };
            string[] separator = null;
            var objects = DisplayUtility.GetCompoents(obj, targetType);
            if (objects != null)
            {
                //Options
                var _options = new List<string>() { "<NULL>" };
                _options.AddRange(DisplayUtility.CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
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
            DisplayUtility.CustomMenu(options, separator, selected, selected, (value) =>
            {
                if (objects != null && value < objects.Length)
                {
                    node.objectReferenceValue = objects[value];
                    //应用更改
                    info.ApplyModifiedProperties();
                }
            });
        }
        public static void PopupArrayComponents(DrawerInfo info, SerializedProperty arrayParent, Type targetType)
        {
            var objDict = new Dictionary<SerializedProperty, Dictionary<string, Object>>();
            var objName = new List<string>();
            for (int i = 0; i < arrayParent.arraySize; i++)
            {
                var node = arrayParent.GetArrayElementAtIndex(i);
                var compoents = new Dictionary<string, Object>();
                var _objects = DisplayUtility.GetCompoents(node.objectReferenceValue, targetType);
                if (_objects != null)
                {
                    Array.ForEach(_objects, o =>
                    {
                        var name = o.GetType().Name;
                        objName.Add(name);
                        compoents[name] = o;
                    });
                }
                objDict.Add(node, compoents);
            }
            objName = objName.Distinct().ToList();
            objName.Sort();
            Array.ForEach(new[] { "Transform", "GameObject" }, name =>
            {
                if (!objName.Contains(name)) return;
                objName.Remove(name);
                objName.Insert(0, name);
            });
            objName.Insert(0, "<NULL>");

            var selected = new int[0];
            var separator = new[] { "" };
            DisplayUtility.CustomMenu(objName.ToArray(), separator, selected, selected, (value) =>
            {
                if (value >= 0 && value < objName.Count)
                {
                    var name = objName[value];
                    foreach (var item in objDict)
                    {
                        Object obj = null;
                        if (item.Value.TryGetValue(name, out obj) || value == 0)
                        {
                            item.Key.objectReferenceValue = obj;
                        }
                    }
                    //应用更改
                    info.ApplyModifiedProperties();
                }
            });
        }

        /// <summary> 检测命名是否符合规则 </summary>
        public static bool CheckKeyValidity(string name)
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
        public static bool CheckFields(Type type)
        {
            FieldInfo index, key;
            return typeof(JsPropertys.IPair).IsAssignableFrom(type)
                && (index = type.GetField("index")) != null && index.FieldType == typeof(int)
                && (key = type.GetField("key")) != null && key.FieldType == typeof(string)
                && type.GetField("value") != null;
        }
        public static void Copy(Element from, Element to)
        {
            if (from.valueType.IsArray && to.valueType.IsArray)
            {
                var targetType = to.valueType.GetElementType();
                for (int i = 0; i < from.valueNode.arraySize && i < to.valueNode.arraySize; i++)
                {
                    Copy(from.valueNode.GetArrayElementAtIndex(i), to.valueNode.GetArrayElementAtIndex(i), targetType);
                }
            }
            else
            {
                Copy(from.valueNode, to.valueNode, to.valueType);
            }
        }
        public static void Copy(SerializedProperty fromNode, SerializedProperty toNode, Type targetType)
        {
            if (fromNode.propertyType == toNode.propertyType)
            {
                switch (fromNode.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        toNode.intValue = fromNode.intValue;
                        break;
                    case SerializedPropertyType.Boolean:
                        toNode.boolValue = fromNode.boolValue;
                        break;
                    case SerializedPropertyType.Float:
                        toNode.doubleValue = fromNode.doubleValue;
                        break;
                    case SerializedPropertyType.String:
                        toNode.stringValue = fromNode.stringValue;
                        break;
                    case SerializedPropertyType.Color:
                        toNode.colorValue = fromNode.colorValue;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        if (fromNode.objectReferenceValue != null && targetType.IsAssignableFrom(fromNode.objectReferenceValue.GetType()))
                            toNode.objectReferenceValue = fromNode.objectReferenceValue;
                        break;
                    case SerializedPropertyType.Vector2:
                        toNode.vector2Value = fromNode.vector2Value;
                        break;
                    case SerializedPropertyType.Vector3:
                        toNode.vector3Value = fromNode.vector3Value;
                        break;
                    case SerializedPropertyType.Vector4:
                        toNode.vector4Value = fromNode.vector4Value;
                        break;
                    case SerializedPropertyType.Rect:
                        toNode.rectValue = fromNode.rectValue;
                        break;
                    case SerializedPropertyType.Bounds:
                        toNode.boundsValue = fromNode.boundsValue;
                        break;
                    case SerializedPropertyType.Vector2Int:
                        toNode.vector2IntValue = fromNode.vector2IntValue;
                        break;
                    case SerializedPropertyType.Vector3Int:
                        toNode.vector3IntValue = fromNode.vector3IntValue;
                        break;
                    case SerializedPropertyType.RectInt:
                        toNode.rectIntValue = fromNode.rectIntValue;
                        break;
                    case SerializedPropertyType.BoundsInt:
                        toNode.boundsIntValue = fromNode.boundsIntValue;
                        break;
                }
            }
        }
        //在鼠标位置弹出菜单 (在GUI之后弹出面板)
        public static void CustomMenu(string[] options, string[] separator, int[] selected, int[] disabled, Action<int> callback)
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
        /// <summary>
        /// 获取UnityEngine.Object上的组件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        static Object[] GetCompoents(Object obj, Type targetType)
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
                //移除无效项
                if (targetType != null)
                {
                    for (int i = result.Count - 1; i >= 0; i--)
                    {
                        if (result[i] == null || !targetType.IsAssignableFrom(result[i].GetType()))
                        {
                            result.RemoveAt(i);
                        }
                    }
                }
                return result.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 检测Menu.Options是否有重复名称, 并返回更正后的Options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary> 删除选中的节点 </summary>
        static void DeleteElements(Element[] elements)
        {
            //依据父节点进行分组
            var grouping = new Dictionary<SerializedProperty, List<Element>>();
            foreach (var element in elements)
            {
                List<Element> list;
                if (!grouping.TryGetValue(element.arrayParent, out list))
                {
                    list = new List<Element>();
                    grouping.Add(element.arrayParent, list);
                }
                list.Add(element);
            }
            //进行排序然后删除(先删除arrayIndex大的节点)
            foreach (var group in grouping)
            {
                var values = group.Value.ToList();
                values.Sort((v1, v2) => v1.arrayIndex > v2.arrayIndex ? -1 : v1.arrayIndex < v2.arrayIndex ? 1 : 0);
                foreach (var element in values)
                {
                    element.DeleteCommand();
                }
            }
        }
        public static string GenCode(JsPropertys.ResultPair[] pairs, string declareType, bool useFullname)
        {
            var resultCode = "";
            declareType = declareType.Trim();
            foreach (var pair in pairs)
            {
                if (!string.IsNullOrEmpty(resultCode))
                    resultCode += "\n";
                var typeStr = GetTypeName(null, useFullname);
                if (pair.value != null)
                {
                    typeStr = GetTypeName(pair.value.GetType(), useFullname);
                    if (pair.value.GetType().IsArray && ((Array)pair.value).Length > 0)
                    {
                        var arr = (Array)pair.value;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var o = arr.GetValue(i);
                            if (o != null && !o.Equals(null))
                            {
                                typeStr = "System.Array$1<" + GetTypeName(o.GetType(), useFullname) + ">";
                                break;
                            }
                        }
                    }
                }
                var keyStr = pair.key;
                if (!CheckKeyValidity(keyStr))
                    keyStr = "[\"" + keyStr + "\"]";
                resultCode += declareType + " " + keyStr + ": " + typeStr + ";";
            }
            return resultCode;
        }
        public static void PraseCode(JsPropertys_CustomEditor editor, string code)
        {
            code = code.Replace("\r", "").Replace("\n", "");
            int valueType = 0;
            var keys = editor.GetElements().Select(o => o.key).ToList();
            foreach (var line in code.Split(';'))
            {
                var columns = line.Trim().Split(' ');
                if (columns.Length < 2 || !columns[1].Contains(":"))
                    return;
                var key = columns[1].Substring(0, columns[1].IndexOf(":"));
                if (keys.Contains(key))
                    continue;
                keys.Add(key);

                //Create Element
                var arrayParent = editor.drawInfo.GetSerializedProperty(editor.drawInfo.typeMapping.Keys.ElementAt(valueType));
                arrayParent.arraySize++;
                var create = new Element(arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1), editor.drawInfo.typeMapping.Values.ElementAt(valueType));
                create.index = keys.Count - 1;
                create.key = key;
                if (create.valueNode.isArray)
                {
                    create.valueNode.arraySize = 0;
                    create.valueNode.isExpanded = true;
                }
                create.Clean();
                //应用更改
                editor.drawInfo.ApplyModifiedProperties();
            }

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

    }

    public abstract class Drawer
    {
#if UNITY_2019_1_OR_NEWER
        protected const string OPTIONS_STYLE = "PaneOptions";
#else
        protected const string OPTIONS_STYLE = "Icon.Options";
#endif

        protected const float HEIGHT_SPACING = HEIGHT_SPACING_HALF * 2;
        protected const float HEIGHT_SPACING_HALF = 2f;
        protected const float OPTIONS_WIDTH = 16f;
        protected const float QUAD_WIDTH = OPTIONS_WIDTH + 4f;
        public Element element { get; set; }
        public DrawerInfo drawInfo { get; set; }

        public virtual void Draw(Rect position, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, element.arrayParent))
            {
                //设置属性名宽度 Name HP
                EditorGUIUtility.labelWidth = 0;
                //输入框高度，默认一行的高度
                position.height = this.GetLineHeight();

                float normalizeWidth = position.width - QUAD_WIDTH,
                    normalizeHalf = normalizeWidth / 2f;

                //输入框的位置
                Rect keyRect = new Rect(position)
                {
                    width = normalizeHalf,
                    height = position.height - HEIGHT_SPACING,
                    y = position.y + HEIGHT_SPACING_HALF
                };
                Rect valueRect = new Rect(keyRect)
                {
                    x = keyRect.x + keyRect.width + 2,
                    width = normalizeHalf,
                };
                Rect optionsRect = new Rect(valueRect)
                {
                    x = valueRect.x + valueRect.width + 2,
                };

                element.key = EditorGUI.TextField(keyRect, string.Empty, element.key);
                DrawValue(position, valueRect);
                DrawOptions(optionsRect);
            }
        }
        public virtual float GetLineHeight()
        {
            return EditorGUIUtility.singleLineHeight + HEIGHT_SPACING;
        }
        public virtual float GetHeight()
        {
            return this.GetLineHeight();
        }

        protected virtual void DrawValue(Rect position, Rect valueRect)
        {
        }
        protected virtual void DrawOptions(Rect optionsRect)
        {
            if (EditorGUI.Toggle(optionsRect, false, OPTIONS_STYLE))
            {
                DisplayUtility.PopupTypes(drawInfo, element);
            }
        }
    }
    public abstract class ArrayDrawer : Drawer
    {
        const float PADDING_LEFT = 0f;
        const float PADDING = OPTIONS_WIDTH;
        private Color gray = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        public override float GetHeight()
        {
            var height = base.GetLineHeight();
            if (element != null)
            {
                var arrayParent = element.valueNode;
                if (arrayParent.isExpanded)
                {
                    height += base.GetLineHeight() * (arrayParent.arraySize + 1) + HEIGHT_SPACING;
                    if (arrayParent.arraySize > 0)
                        height += HEIGHT_SPACING;
                }
            }
            return height;
        }
        protected override void DrawValue(Rect position, Rect valueRect)
        {
            //Array
            var arrayParent = element.valueNode;

            var title = element.valueType.Name;
            if (title.EndsWith("[]"))
                title = title.Replace("[]", "[" + arrayParent.arraySize + "]");
            var newState = EditorGUI.Foldout(new Rect(valueRect) { x = valueRect.x + 10 }, arrayParent.isExpanded, title);
            if (newState != arrayParent.isExpanded)
            {
                arrayParent.isExpanded = !arrayParent.isExpanded;
            }
            if (arrayParent.isExpanded)
            {
                float lineHight = base.GetLineHeight();

                //draw background
                Rect bgRect = new Rect(position)
                {
                    y = lineHight + position.y,
                    x = position.x + PADDING_LEFT,
                    width = position.width - PADDING,
                    height = lineHight * (arrayParent.arraySize + 1)
                };
                if (arrayParent.arraySize > 0)
                    bgRect.height += HEIGHT_SPACING;
                EditorGUI.DrawRect(bgRect, gray);

                //draw title
                Rect titleRect = new Rect(position)
                {
                    y = position.y + lineHight,
                    x = position.x + PADDING_LEFT,
                    width = position.width - PADDING
                };

                //draw elements
                bool hasObjectReference = false;
                Rect elementRect = new Rect(titleRect)
                {
                    y = titleRect.y + titleRect.height
                };
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    hasObjectReference |= DrawElement(new Rect(elementRect) { y = elementRect.y + lineHight * i }
                            , i, arrayParent.GetArrayElementAtIndex(i));
                }

                DrawTitle(titleRect, arrayParent, hasObjectReference);
            }
        }
        protected abstract void DrawArrayElement(Rect positon, SerializedProperty node, Type type);

        private bool DrawElement(Rect position, int index, SerializedProperty node)
        {
            Rect indexRect = new Rect(position)
            {
                width = 60f,
            },
                valueRect = new Rect(indexRect)
                {
                    x = indexRect.x + indexRect.width + 2,
                    width = position.width - indexRect.width - QUAD_WIDTH
                },
                optionsRect = new Rect(valueRect)
                {
                    x = valueRect.x + valueRect.width + 2,
                    width = QUAD_WIDTH,
                };

            EditorGUI.LabelField(indexRect, $"  [{index.ToString().PadLeft(3, ' ')}]");
            DrawArrayElement(valueRect, node, element.valueType.GetElementType());

            if (node.propertyType == SerializedPropertyType.ObjectReference
                       && EditorGUI.Toggle(optionsRect, false, OPTIONS_STYLE))
            {
                DisplayUtility.PopupComponents(drawInfo, node, node.objectReferenceValue, element.valueType.GetElementType());
            }
            return node.propertyType == SerializedPropertyType.ObjectReference;
        }
        private void DrawTitle(Rect position, SerializedProperty parentNode, bool hasObjectReference)
        {
            float height = EditorGUIUtility.singleLineHeight;
            Rect mbRect = new Rect(position)
            {
                x = position.x + position.width - PADDING - height * 2 - HEIGHT_SPACING * 2,
                height = height,
                width = height
            };
            Rect pbRect = new Rect(mbRect)
            {
                x = mbRect.x + height + HEIGHT_SPACING
            };

            if (EditorGUI.Toggle(mbRect, false, "OL Minus") && parentNode.arraySize > 0)
            {
                parentNode.arraySize--;
            }
            if (EditorGUI.Toggle(pbRect, false, "OL Plus"))
            {
                parentNode.arraySize++;
            }

            if (hasObjectReference)
            {
                Rect optionsRect = new Rect(pbRect) { x = pbRect.x + height + HEIGHT_SPACING };
                if (EditorGUI.Toggle(optionsRect, false, OPTIONS_STYLE))
                {
                    DisplayUtility.PopupArrayComponents(drawInfo, parentNode, element.valueType.GetElementType());
                }
            }
        }
    }

    class NumberDrawer : Drawer
    {
        protected override void DrawValue(Rect position, Rect valueRect)
        {
            element.valueNode.doubleValue = EditorGUI.DoubleField(valueRect, string.Empty, element.valueNode.doubleValue);
        }
    }
    class StringDrawer : Drawer
    {
        protected override void DrawValue(Rect position, Rect valueRect)
        {
            element.valueNode.stringValue = EditorGUI.TextField(valueRect, string.Empty, element.valueNode.stringValue);
        }
    }
    class BooleanDrawer : Drawer
    {
        protected override void DrawValue(Rect position, Rect valueRect)
        {
            element.valueNode.boolValue = EditorGUI.Toggle(valueRect, string.Empty, element.valueNode.boolValue);
        }
    }
    class ObjectDrawer : Drawer
    {
        protected override void DrawValue(Rect position, Rect valueRect)
        {
            element.valueNode.objectReferenceValue = EditorGUI.ObjectField(valueRect, string.Empty, element.valueNode.objectReferenceValue, typeof(Object), true);
        }
        protected override void DrawOptions(Rect optionsRect)
        {
            if (EditorGUI.Toggle(optionsRect, false, OPTIONS_STYLE))
            {
                DisplayUtility.PopupComponentsAndTypes(drawInfo, element, element.valueNode.objectReferenceValue, element.valueType);
            }
        }
    }
    class ObjectArrayDrawer : ArrayDrawer
    {
        protected override void DrawArrayElement(Rect valueRect, SerializedProperty node, Type type)
        {
            node.objectReferenceValue = EditorGUI.ObjectField(valueRect, string.Empty, node.objectReferenceValue, typeof(Object), true);
        }
    }
    class GameObjectArrayDrawer : ArrayDrawer
    {
        protected override void DrawArrayElement(Rect valueRect, SerializedProperty node, Type type)
        {
            node.objectReferenceValue = EditorGUI.ObjectField(valueRect, string.Empty, node.objectReferenceValue, typeof(Object), true);
        }
    }
}
#endif