namespace Puerts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class RefUtils
    {
        /// <summary>
        /// 保存方法映射
        /// </summary>
        const bool EnableMapping = true;
        /// <summary>
        /// JsEnv引用管理器列表
        /// </summary>
        static List<GlobalRef> globalRefs = new List<GlobalRef>();

        static GlobalRef Ref(JsEnv env)
        {
            foreach (var r in globalRefs.ToArray())
            {
                if (r.IsAlive)
                {
                    if (r.env == env)
                        return r;
                }
                else
                    globalRefs.Remove(r);
            }
            var _ref = new GlobalRef(env)
            {
                mapping = EnableMapping
            };
            globalRefs.Add(_ref);
            return _ref;
        }

        public static bool IsAlive(JsEnv env)
        {
            return env != null && env.isolate != IntPtr.Zero;
        }
        public static TResult Load<TResult>(JsEnv env, string chunk, params object[] args)
        {
            return Ref(env).Load<TResult>(chunk, args);
        }
        public static void Load(JsEnv env, string chunk, params object[] args)
        {
            Ref(env).Load(chunk, args);
        }
        public static JsObject NewObject(JsEnv env)
        {
            return Ref(env).NewObject();
        }
        public static string Value(JsEnv env, string id)
        {
            return Ref(env).Value(id);
        }

        public static TResult Get<TKey, TResult>(JsEnv env, string id, TKey key)
        {
            return Ref(env).Get<TKey, TResult>(id, key);
        }
        public static bool Set<TKey, TValue>(JsEnv env, string id, TKey key, TValue value)
        {
            return Ref(env).Set<TKey, TValue>(id, key, value);
        }
        public static TResult GetInPath<TResult>(JsEnv env, string id, string path)
        {
            return Ref(env).GetInPath<TResult>(id, path);
        }
        public static bool SetInPath<TValue>(JsEnv env, string id, string path, TValue value)
        {
            return Ref(env).SetInPath<TValue>(id, path, value);
        }
        public static bool IsCast(JsEnv env, string id, object key, Type type)
        {
            return Ref(env).IsCast(id, key, type);
        }
        public static bool IsCastInPath(JsEnv env, string id, string path, Type type)
        {
            return Ref(env).IsCastInPath(id, path, type);
        }

        public static bool Contains(JsEnv env, string id, string path)
        {
            return Ref(env).Contains(id, path);
        }
        public static string[] Keys(JsEnv env, string id)
        {
            return Ref(env).Keys(id);
        }
        public static int Length(JsEnv env, string id)
        {
            return Ref(env).Length(id);
        }
        public static void ForEach<TValue>(JsEnv env, string id, Action<string, TValue> action)
        {
            Ref(env).ForEach<TValue>(id, action);
        }
        public static void Call(JsEnv env, string id, string name, params object[] args)
        {
            Ref(env).Call(id, name, args);
        }
        public static T Call<T>(JsEnv env, string id, string name, params object[] args)
        {
            return Ref(env).Call<T>(id, name, args);
        }

        public static bool Ref(JsEnv env, string id)
        {
            return Ref(env).Ref(id);
        }
        public static bool Release(JsEnv env, string id, int count = 1)
        {
            return Ref(env).Release(id, count);
        }
        public static bool Delete(JsEnv env, string id)
        {
            return Ref(env).Delete(id);
        }

        private delegate void LoadAction(params object[] args);
        private delegate TResult LoadFunc<TResult>(params object[] args);

        private class GlobalRef
        {
            public WeakReference weakref { get; private set; }
            public bool IsAlive { get { return weakref.IsAlive; } }
            public bool mapping
            {
                get { return _mapping; }
                set
                {
                    _mapping = value;
                    if (!_mapping)
                    {
                        _ref = null;
                        _delete = null;
                        _release = null;
                        _contains = null;
                        _setter.Clear();
                        _getter.Clear();
                        _setterInPath.Clear();
                        _getterInPath.Clear();
                        _cast = null;
                        _castInPath = null;
                        _keys = null;
                        _length = null;
                    }
                }
            }
            public JsEnv env
            {
                get
                {
                    if (!weakref.IsAlive)
                        throw new Exception("JsEnv is release.");
                    var e = weakref.Target as JsEnv;
                    if (e == null || e.isolate == IntPtr.Zero)
                        throw new Exception("JsEnv is closed.");
                    return e;
                }
            }
            private bool _mapping { get; set; }
            private List<Type> usingTypes = new List<Type>();
            //Mapping Func
            private Func<string, bool> _ref;
            private Func<string, bool> _delete;
            private Func<string, int, bool> _release;
            private Func<string, object, bool> _contains;
            private Dictionary<Type, object> _setter = new Dictionary<Type, object>();
            private Dictionary<Type, object> _getter = new Dictionary<Type, object>();
            private Dictionary<Type, object> _setterInPath = new Dictionary<Type, object>();
            private Dictionary<Type, object> _getterInPath = new Dictionary<Type, object>();
            private Func<string, object, Type, bool> _cast;
            private Func<string, string, Type, bool> _castInPath;
            private Func<string, string[]> _keys;
            private Func<string, int> _length;

            public GlobalRef(JsEnv env)
            {
                weakref = new WeakReference(env);
                Ready();
            }
            ~GlobalRef()
            {
                mapping = false;
                usingTypes.Clear();
            }

            public TResult Load<TResult>(string chunk, params object[] args)
            {
                var script = @"
(args) => { 
    var chunk = (function(){ " + chunk + @" })();
    if (typeof (chunk) === 'function') {
        var _args = [];
        for (var i = 0; i < args.Length; i++)
            _args.push(args[i]);
        return $puertsRef.toCSharp(chunk(..._args)); 
    }
    return $puertsRef.toCSharp(chunk); 
}";
                UsingFunc<object[], TResult>();
                var load = env.Eval<LoadFunc<TResult>>(script);
                if (load != null)
                {
                    return load(args);
                }
                return default;
            }
            public void Load(string chunk, params object[] args)
            {
                var script = @"
(args) => { 
    var chunk = (function(){ " + chunk + @" })();
    if (typeof (chunk) === 'function') {
        var _args = [];
        for (var i = 0; i < args.Length; i++)
            _args.push(args[i]);
        chunk(..._args);
    }
}";
                UsingAction<object[]>();
                var load = env.Eval<LoadAction>(script);
                if (load != null)
                {
                    load(args);
                }
            }
            public JsObject NewObject()
            {
                return env.Eval<JsObject>("$puertsRef.toCSharp({})");
            }
            public string Value(string id)
            {
                return string.Format("$puertsRef.value('{0}')", id);
            }

            public TResult Get<TKey, TResult>(string id, TKey key)
            {
                if (IsCast(id, key, typeof(TResult)))
                {
                    var getter = Getter<TKey, TResult>();
                    if (getter != null)
                    {
                        var result = getter(id, key);
                        if (result != null)
                            return (TResult)result;
                    }
                    return default;
                }
                throw new InvalidCastException("");
            }
            public bool Set<TKey, TValue>(string id, TKey key, TValue value)
            {
                var setter = Setter<TKey, TValue>();
                if (setter != null)
                {
                    return setter(id, key, value);
                }
                return false;
            }
            public TResult GetInPath<TResult>(string id, string path)
            {
                if (IsCastInPath(id, path, typeof(TResult)))
                {
                    var getter = GetterInPath<TResult>();
                    if (getter != null)
                    {
                        var result = getter(id, path);
                        if (result != null)
                            return (TResult)result;
                    }
                    return default;
                }
                throw new InvalidCastException("");
            }
            public bool SetInPath<TValue>(string id, string path, TValue value)
            {
                var setter = SetterInPath<TValue>();
                if (setter != null)
                {
                    return setter(id, path, value);
                }
                return false;
            }
            public bool IsCast(string id, object key, Type type)
            {
                UsingFunc<string, object, Type, bool>();
                var cast = _cast ?? env.Eval<Func<string, object, Type, bool>>("$puertsRef.isCast");
                if (mapping) _cast = cast;
                if (cast != null)
                {
                    return cast(id, key, type);
                }
                return false;
            }
            public bool IsCastInPath(string id, string path, Type type)
            {
                UsingFunc<string, string, Type, bool>();
                var cast = _castInPath ?? env.Eval<Func<string, string, Type, bool>>("$puertsRef.isCastInPath");
                if (mapping) _castInPath = cast;
                if (cast != null)
                {
                    return cast(id, path, type);
                }
                return false;
            }

            public bool Contains(string id, string path)
            {
                UsingFunc<string, string, bool>();
                var contains = _contains ?? env.Eval<Func<string, object, bool>>("$puertsRef.contains");
                if (mapping) _contains = contains;
                if (contains != null)
                {
                    return contains(id, path);
                }
                return false;
            }
            public string[] Keys(string id)
            {
                UsingFunc<string, string[]>();
                var keys = _keys ?? env.Eval<Func<string, string[]>>("$puertsRef.keys");
                if (mapping) _keys = keys;
                if (keys != null)
                {
                    return keys(id);
                }
                return null;
            }
            public int Length(string id)
            {
                UsingFunc<string, int>();
                var len = _length ?? env.Eval<Func<string, int>>("$puertsRef.length");
                if (mapping) _length = len;
                if (len != null)
                {
                    return len(id);
                }
                return -1;
            }
            public void ForEach<TValue>(string id, Action<string, TValue> action)
            {
                UsingAction<string, Action<string, TValue>, Type>();
                var _foreach = env.Eval<Action<string, Action<string, TValue>, Type>>("$puertsRef.foreach");
                if (_foreach != null)
                {
                    _foreach(id, action, typeof(TValue));
                }
            }
            public void Call(string id, string name, params object[] args)
            {
                UsingAction<string, string, object[]>();
                var call = env.Eval<Action<string, string, object[]>>("$puertsRef.call");
                if (call != null)
                {
                    call(id, name, args);
                }
            }
            public T Call<T>(string id, string name, params object[] args)
            {
                UsingFunc<string, string, object[], T>();
                var call = env.Eval<Func<string, string, object[], T>>("$puertsRef.call");
                if (call != null)
                {
                    var result = call(id, name, args);
                    if (result != null
                        && typeof(T).IsAssignableFrom(result.GetType()))
                        return (T)result;
                }
                return default;
            }


            public bool Ref(string id)
            {
                if (mapping)
                {
                    if (_ref == null)
                    {
                        UsingFunc<string, bool>();
                        _ref = env.Eval<Func<string, bool>>("$puertsRef.ref");
                    }
                    return _ref(id);
                }
                return env.Eval<bool>(string.Format("$puertsRef.ref({0})", id));
            }
            public bool Delete(string id)
            {
                if (mapping)
                {
                    if (_delete == null)
                    {
                        UsingFunc<string, bool>();
                        _delete = env.Eval<Func<string, bool>>("$puertsRef.delete");
                    }
                    return _delete(id);
                }
                return env.Eval<bool>(string.Format("$puertsRef.delete({0})", id));
            }
            public bool Release(string id, int count = 1)
            {
                if (mapping)
                {
                    if (_release == null)
                    {
                        UsingFunc<string, int, bool>();
                        _release = env.Eval<Func<string, int, bool>>("$puertsRef.release");
                    }
                    return _release(id, count);
                }
                return env.Eval<bool>(string.Format("$puertsRef.release({0},{1})", id, count));
            }

            Func<string, TKey, TResult> Getter<TKey, TResult>()
            {
                object func;
                if (!_getter.TryGetValue(typeof(Action<TKey, TResult>), out func))
                {
                    UsingFunc<string, TKey, TResult>();
                    func = env.Eval<Func<string, TKey, TResult>>("$puertsRef.get");
                    if (mapping)
                        _getter.Add(typeof(Action<TKey, TResult>), func);
                }
                if (func == null)
                    return null;
                return (Func<string, TKey, TResult>)func;
            }
            Func<string, TKey, TValue, bool> Setter<TKey, TValue>()
            {
                object action;
                if (!_setter.TryGetValue(typeof(Action<TKey, TValue>), out action))
                {
                    UsingFunc<string, TKey, TValue, bool>();
                    action = env.Eval<Func<string, TKey, TValue, bool>>("$puertsRef.set");
                    if (mapping)
                        _setter.Add(typeof(Action<TKey, TValue>), action);
                }
                if (action == null)
                    return null;
                return (Func<string, TKey, TValue, bool>)action;
            }
            Func<string, string, TResult> GetterInPath<TResult>()
            {
                object func;
                if (!_getterInPath.TryGetValue(typeof(TResult), out func))
                {
                    UsingFunc<string, string, TResult, bool>();
                    func = env.Eval<Func<string, string, TResult>>("$puertsRef.getInPath");
                    if (mapping)
                        _getterInPath.Add(typeof(TResult), func);
                }
                if (func == null)
                    return null;
                return (Func<string, string, TResult>)func;
            }
            Func<string, string, TValue, bool> SetterInPath<TValue>()
            {
                object action;
                if (!_setterInPath.TryGetValue(typeof(TValue), out action))
                {
                    UsingFunc<string, string, TValue, bool>();
                    action = env.Eval<Func<string, string, TValue, bool>>("$puertsRef.setInPath");
                    if (mapping)
                        _setterInPath.Add(typeof(TValue), action);
                }
                if (action == null)
                    return null;
                return (Func<string, string, TValue, bool>)action;
            }
            void Ready()
            {
                bool exist = env.Eval<bool>("var ref = globalThis['$puertsRef']; ref !== undefined && ref !== null && ref !== void 0");
                if (!exist)
                {
                    env.Eval(ready);
                    env.Eval<Action<JsEnv>>("$puertsRef.init")(env);
                }
            }
            void UsingAction<T1>()
            {
                var type = typeof(Action<T1>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingAction<T1>();
                }
            }
            void UsingAction<T1, T2>()
            {
                var type = typeof(Action<T1, T2>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingAction<T1, T2>();
                }
            }
            void UsingAction<T1, T2, T3>()
            {
                var type = typeof(Action<T1, T2, T3>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingAction<T1, T2, T3>();
                }
            }
            void UsingAction<T1, T2, T3, T4>()
            {
                var type = typeof(Action<T1, T2, T3, T4>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingAction<T1, T2, T3, T4>();
                }
            }
            void UsingFunc<T1, TResult>()
            {
                var type = typeof(Func<T1, TResult>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingFunc<T1, TResult>();
                }
            }
            void UsingFunc<T1, T2, TResult>()
            {
                var type = typeof(Func<T1, T2, TResult>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingFunc<T1, T2, TResult>();
                }
            }
            void UsingFunc<T1, T2, T3, TResult>()
            {
                var type = typeof(Func<T1, T2, T3, TResult>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingFunc<T1, T2, T3, TResult>();
                }
            }
            void UsingFunc<T1, T2, T3, T4, TResult>()
            {
                var type = typeof(Func<T1, T2, T3, T4, TResult>);
                if (!usingTypes.Contains(type))
                {
                    usingTypes.Add(type);
                    env.UsingFunc<T1, T2, T3, T4, TResult>();
                }
            }
        }

        //const string ready = "require('./global');";
        const string ready = @"
const CS = require('csharp');
const puerts_1 = require('puerts');
function isCSharp(value) {
    return typeof (value['GetType']) !== 'undefined'
        && typeof (value['GetHashCode']) !== 'undefined';
}
function isValue(value) {
    var type = typeof (value);
    return type === 'string' || type === 'number' || type === 'boolean';
}
const numbers = [
    puerts_1.$typeof(CS.System.Int16), puerts_1.$typeof(CS.System.Int32), puerts_1.$typeof(CS.System.Int64),
    puerts_1.$typeof(CS.System.Single), puerts_1.$typeof(CS.System.Double), puerts_1.$typeof(CS.System.UInt16),
    puerts_1.$typeof(CS.System.UInt32), puerts_1.$typeof(CS.System.UInt64)
];
function isNumber(type) {
    for (let i = 0; i < numbers.length; i++)
        if (numbers[i].Equals(type))
            return true;
    return false;
}
function isCast(value, type) {
    if (isCSharp(value)) {
        return puerts_1.$typeof(CS.System.Object).Equals(type)
            || type.IsAssignableFrom(value.GetType());
    }
    if (isValue(value)) {
        var v_type = typeof (value);
        return puerts_1.$typeof(CS.System.Object).Equals(type)
            || v_type === 'number' && isNumber(type)
            || v_type === 'string' && puerts_1.$typeof(CS.System.String).Equals(type)
            || v_type === 'boolean' && puerts_1.$typeof(CS.System.Boolean).Equals(type);
    }
    if (typeof (value) == 'function') {
        return puerts_1.$typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
    }
    var j_type = puerts_1.$typeof(CS.Puerts.JsBase);
    while (type != null) {
        if (j_type.Equals(type))
            return true;
        type = type.BaseType;
    }
    return puerts_1.$typeof(CS.System.Object).Equals(type);
}
class GlobalRef {
    constructor() {
        this.globalIdx = 0;
        this.globalEnv = null;
        this.init = (env) => {
            this.globalIdx = 0;
            this.globalEnv = env;
            this.poolIds = new Map();
            this.poolObjs = new WeakMap();
        };
        this.create = (obj) => {
            let id = this.poolObjs.get(obj);
            if (id === undefined || id === null || id === void 0) {
                id = this.globalEnv.Index + '_' + (++this.globalIdx).toString();
                this.poolIds.set(id, { value: obj, count: 0 });
                this.poolObjs.set(obj, id);
            }
            return id;
        };
        this.ref = (id) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                ref.count++;
                return true;
            }
            return false;
        };
        this.release = (id, count) => {
            count = count !== null && count !== void 0 ? count : 1;
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                ref.count -= count;
                if (ref.count <= 0) {
                    this.poolIds.delete(id);
                    this.poolObjs.delete(ref.value);
                }
                return true;
            }
            return false;
        };
        this.delete = (obj) => {
            if (obj === undefined || obj === null || obj === void 0)
                throw new Error('invalid parameters obj');
            let id = typeof (obj) === 'string' ? obj : this.poolObjs.get(obj);
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                this.poolIds.delete(id);
                this.poolObjs.delete(ref.value);
                return true;
            }
            return false;
        };
        this.toCSharp = (obj) => {
            if (obj === undefined || obj === null || obj === void 0)
                return null;
            if (typeof (obj) === 'function'
                || isValue(obj) || isCSharp(obj))
                return obj;
            return new CS.Puerts.JsObject(this.create(obj), this.globalEnv);
        };
        this.value = (id) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                return ref.value;
            }
            return undefined;
        };
        this.get = (id, key) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                return this.toCSharp(ref.value[key]);
            }
            return null;
        };
        this.set = (id, key, value) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                ref.value[key] = value;
                return true;
            }
            return false;
        };
        this.getInPath = (id, path) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let tmp = ref.value;
                let nodes = path.split('.');
                let last = nodes.pop();
                nodes.forEach(n => {
                    tmp = tmp[n];
                    if (typeof (tmp) === 'undefined')
                        throw new Error('invalid path: ' + path);
                });
                return this.toCSharp(tmp[last]);
            }
            return null;
        };
        this.setInPath = (id, path, value) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let tmp = ref.value;
                let nodes = path.split('.');
                let last = nodes.pop();
                nodes.forEach(n => {
                    tmp = tmp[n];
                    if (typeof (tmp) === 'undefined')
                        throw new Error('invalid path: ' + path);
                });
                tmp[last] = value;
                return true;
            }
            return false;
        };
        this.isCast = (id, key, type) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                var v = ref.value[key];
                if (v === undefined || v === null || v === void 0)
                    return type.IsClass;
                return isCast(v, type);
            }
            return false;
        };
        this.isCastInPath = (id, path, type) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let tmp = ref.value;
                let nodes = path.split('.');
                let last = nodes.pop();
                for (let i = 0; i < nodes.length; i++) {
                    tmp = tmp[nodes[i]];
                    if (typeof (tmp) === 'undefined')
                        return false;
                }
                var v = tmp[last];
                if (v === undefined || v === null || v === void 0)
                    return type.IsClass;
                return isCast(v, type);
            }
            return false;
        };
        this.contains = (id, path) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let tmp = ref.value;
                let nodes = path.split('.');
                let last = nodes.pop();
                for (let i = 0; i < nodes.length; i++) {
                    tmp = tmp[nodes[i]];
                    if (typeof (tmp) === 'undefined')
                        return false;
                }
                return typeof (tmp[last]) !== 'undefined';
            }
            return false;
        };
        this.keys = (id) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let keys = Object.keys(ref.value);
                let cs_keys = CS.System.Array.CreateInstance(puerts_1.$typeof(CS.System.String), keys.length);
                for (let i = 0; i < keys.length; i++) {
                    cs_keys.SetValue(keys[i], i);
                }
                return cs_keys;
            }
            return null;
        };
        this.length = (id) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                return Object.keys(ref.value).length;
            }
            return -1;
        };
        this.foreach = (id, action, valueType) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let value = ref.value;
                if (isCSharp(action)
                    && puerts_1.$typeof(CS.System.MulticastDelegate).IsAssignableFrom(action.GetType())) {
                    const func = action;
                    action = (...args) => func.Invoke(...args);
                }
                Object.keys(value).forEach(key => {
                    let v = value[key];
                    if (valueType === undefined || isCast(v, valueType)) {
                        action(key, this.toCSharp(v));
                    }
                });
                return true;
            }
            return false;
        };
        this.call = (id, name, args) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                let func = ref.value[name];
                if (typeof (func) !== 'function')
                    throw new Error(name + ' is not funtion');
                let _args = [];
                for (let i = 0; i < args.length; i++)
                    _args.push(args[i]);
                var result = func.call(ref.value, ..._args);
                return this.toCSharp(result);
            }
            return null;
        };
    }
}
globalThis.$puertsRef = new GlobalRef();
";
    }

    public static class RefExtend
    {
        public static bool IsAlive(this JsEnv env)
        {
            return RefUtils.IsAlive(env);
        }
        public static TResult Load<TResult>(this JsEnv env, string chunk, params object[] args)
        {
            return RefUtils.Load<TResult>(env, chunk, args);
        }
        public static void Load(this JsEnv env, string chunk, params object[] args)
        {
            RefUtils.Load(env, chunk, args);
        }
        public static JsObject NewObject(this JsEnv env)
        {
            return RefUtils.NewObject(env);
        }
    }
}