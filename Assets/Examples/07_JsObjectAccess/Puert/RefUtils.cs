namespace Puerts
{
    using System;
    using System.Collections.Generic;

    public static class RefUtils
    {
        /// <summary>
        /// 保存方法映射
        /// </summary>
        const bool EnableMapping = false;
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
        public static string Value(int ref_id)
        {
            return string.Format("$puertsRef.value({0})", ref_id);
        }

        public static TResult Get<TKey, TResult>(JsEnv env, int ref_id, TKey key)
        {
            return Ref(env).Get<TKey, TResult>(ref_id, key);
        }
        public static bool Set<TKey, TValue>(JsEnv env, int ref_id, TKey key, TValue value)
        {
            return Ref(env).Set<TKey, TValue>(ref_id, key, value);
        }
        public static TResult GetInPath<TResult>(JsEnv env, int ref_id, string path)
        {
            return Ref(env).GetInPath<TResult>(ref_id, path);
        }
        public static bool SetInPath<TValue>(JsEnv env, int ref_id, string path, TValue value)
        {
            return Ref(env).SetInPath<TValue>(ref_id, path, value);
        }

        public static bool Contains(JsEnv env, int ref_id, string path)
        {
            return Ref(env).Contains(ref_id, path);
        }

        public static bool Ref(JsEnv env, int ref_id)
        {
            return Ref(env).Ref(ref_id);
        }
        public static bool Release(JsEnv env, int ref_id, int count = 1)
        {
            return Ref(env).Release(ref_id, count);
        }
        public static bool Delete(JsEnv env, int ref_id)
        {
            return Ref(env).Delete(ref_id);
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
                        _cast = null;
                        _setter.Clear();
                        _getter.Clear();
                        _castInPath = null;
                        _setterInPath.Clear();
                        _getterInPath.Clear();
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
            private Func<int, bool> _ref;
            private Func<int, bool> _delete;
            private Func<int, int, bool> _release;
            private Func<int, object, bool> _contains;
            private Func<int, object, Type, bool> _cast;
            private Dictionary<Type, object> _setter = new Dictionary<Type, object>();
            private Dictionary<Type, object> _getter = new Dictionary<Type, object>();
            private Func<int, string, Type, bool> _castInPath;
            private Dictionary<Type, object> _setterInPath = new Dictionary<Type, object>();
            private Dictionary<Type, object> _getterInPath = new Dictionary<Type, object>();

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
                var get_script = @"
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
                //UnityEngine.Debug.Log(get_script);
                UsingFunc<object[], TResult>();
                var load = env.Eval<LoadFunc<TResult>>(get_script);
                if (load != null)
                {
                    return load(args);
                }
                return default;
            }
            public void Load(string chunk, params object[] args)
            {
                var get_script = @"
(args) => { 
    var chunk = (function(){ " + chunk + @" })();
    if (typeof (chunk) === 'function') {
        var _args = [];
        for (var i = 0; i < args.Length; i++)
            _args.push(args[i]);
        chunk(..._args);
    }
}";
                //UnityEngine.Debug.Log(get_script);
                UsingAction<object[]>();
                var load = env.Eval<LoadAction>(get_script);
                if (load != null)
                {
                    load(args);
                }
            }
            public JsObject NewObject()
            {
                return env.Eval<JsObject>("$puertsRef.toCSharp({})");
            }

            public TResult Get<TKey, TResult>(int ref_id, TKey key)
            {
                UsingFunc<int, object, Type, bool>();
                var cast = _cast ?? env.Eval<Func<int, object, Type, bool>>("$puertsRef.isCast");
                if (mapping) _cast = cast;
                if (cast != null)
                {
                    if (!cast(ref_id, key, typeof(TResult)))
                        throw new Exception("Invalid conversion type");

                    var getter = Getter<TKey, TResult>();
                    if (getter != null)
                    {
                        var result = getter(ref_id, key);
                        if (result == null)
                            return default;

                        return (TResult)result;
                    }
                }
                return default;
            }
            public bool Set<TKey, TValue>(int ref_id, TKey key, TValue value)
            {
                var setter = Setter<TKey, TValue>();
                if (setter != null)
                {
                    return setter(ref_id, key, value);
                }
                return false;
            }
            public TResult GetInPath<TResult>(int ref_id, string path)
            {
                UsingFunc<int, string, Type, bool>();
                var cast = _castInPath ?? env.Eval<Func<int, string, Type, bool>>("$puertsRef.isCastInPath");
                if (mapping) _castInPath = cast;
                if (cast != null)
                {
                    if (!cast(ref_id, path, typeof(TResult)))
                        throw new Exception("Invalid conversion type");

                    var getter = GetterInPath<TResult>();
                    if (getter != null)
                    {
                        var result = getter(ref_id, path);
                        if (result == null)
                            return default;

                        return (TResult)result;
                    }
                }
                return default;
            }
            public bool SetInPath<TValue>(int ref_id, string path, TValue value)
            {
                var setter = SetterInPath<TValue>();
                if (setter != null)
                {
                    return setter(ref_id, path, value);
                }
                return false;
            }

            public bool Contains(int ref_id, string path)
            {
                UsingFunc<int, string, bool>();
                var contains = _contains ?? env.Eval<Func<int, object, bool>>("$puertsRef.contains");
                if (mapping) _contains = contains;
                if (contains != null)
                {
                    return contains(ref_id, path);
                }
                return false;
            }
            public bool Ref(int ref_id)
            {
                if (mapping)
                {
                    if (_ref == null)
                    {
                        UsingFunc<int, bool>();
                        _ref = env.Eval<Func<int, bool>>("$puertsRef.ref");
                    }
                    return _ref(ref_id);
                }
                return env.Eval<bool>(string.Format("$puertsRef.ref({0})", ref_id));
            }
            public bool Delete(int ref_id)
            {
                if (mapping)
                {
                    if (_delete == null)
                    {
                        UsingFunc<int, bool>();
                        _delete = env.Eval<Func<int, bool>>("$puertsRef.delete");
                    }
                    return _delete(ref_id);
                }
                return env.Eval<bool>(string.Format("$puertsRef.delete({0})", ref_id));
            }
            public bool Release(int ref_id, int count = 1)
            {
                if (mapping)
                {
                    if (_release == null)
                    {
                        UsingFunc<int, int, bool>();
                        _release = env.Eval<Func<int, int, bool>>("$puertsRef.release");
                    }
                    return _release(ref_id, count);
                }
                return env.Eval<bool>(string.Format("$puertsRef.release({0},{1})", ref_id, count));
            }


            Func<int, TKey, TResult> Getter<TKey, TResult>()
            {
                object func;
                if (!_getter.TryGetValue(typeof(Action<TKey, TResult>), out func))
                {
                    UsingFunc<int, TKey, TResult>();
                    func = env.Eval<Func<int, TKey, TResult>>("$puertsRef.get");
                    if (mapping)
                        _getter.Add(typeof(Action<TKey, TResult>), func);
                }
                if (func == null)
                    return null;
                return (Func<int, TKey, TResult>)func;
            }
            Func<int, TKey, TValue, bool> Setter<TKey, TValue>()
            {
                object action;
                if (!_setter.TryGetValue(typeof(Action<TKey, TValue>), out action))
                {
                    UsingFunc<int, TKey, TValue, bool>();
                    action = env.Eval<Func<int, TKey, TValue, bool>>("$puertsRef.set");
                    if (mapping)
                        _setter.Add(typeof(Action<TKey, TValue>), action);
                }
                if (action == null)
                    return null;
                return (Func<int, TKey, TValue, bool>)action;
            }
            Func<int, string, TResult> GetterInPath<TResult>()
            {
                object func;
                if (!_getterInPath.TryGetValue(typeof(TResult), out func))
                {
                    UsingFunc<int, string, TResult, bool>();
                    func = env.Eval<Func<int, string, TResult>>("$puertsRef.getInPath");
                    if (mapping)
                        _getterInPath.Add(typeof(TResult), func);
                }
                if (func == null)
                    return null;
                return (Func<int, string, TResult>)func;
            }
            Func<int, string, TValue, bool> SetterInPath<TValue>()
            {
                object action;
                if (!_setterInPath.TryGetValue(typeof(TValue), out action))
                {
                    UsingFunc<int, string, TValue, bool>();
                    action = env.Eval<Func<int, string, TValue, bool>>("$puertsRef.setInPath");
                    if (mapping)
                        _setterInPath.Add(typeof(TValue), action);
                }
                if (action == null)
                    return null;
                return (Func<int, string, TValue, bool>)action;
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
function isCSharpType(arg) {
    return typeof (arg['GetType']) !== 'undefined'
        && typeof (arg['GetHashCode']) !== 'undefined';
}
function isValueType(arg) {
    var type = typeof (arg);
    return type === 'string' || type === 'number' || type === 'boolean';
}
const types = [
    puerts_1.$typeof(CS.System.Int16), puerts_1.$typeof(CS.System.Int32), puerts_1.$typeof(CS.System.Int64),
    puerts_1.$typeof(CS.System.Single), puerts_1.$typeof(CS.System.Double), puerts_1.$typeof(CS.System.UInt16),
    puerts_1.$typeof(CS.System.UInt32), puerts_1.$typeof(CS.System.UInt64)
];
function isNumber(type) {
    for (let i = 0; i < types.length; i++)
        if (types[i].Equals(type))
            return true;
    return false;
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
                id = ++this.globalIdx;
                this.poolIds.set(id, { value: obj, count: 0 });
                this.poolObjs.set(obj, id);
                if (this.globalIdx >= Number.MAX_VALUE)
                    this.globalIdx = Number.MIN_VALUE;
            }
            return { id: id, ref: this.poolIds.get(id) };
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
            let id = typeof (obj) === 'number' ? obj : this.poolObjs.get(obj);
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
            if (isValueType(obj) || isCSharpType(obj))
                return obj;
            if (typeof (obj) === 'function')
                return obj;
            let { id, ref } = this.create(obj);
            return new CS.Puerts.JsObject(id, this.globalEnv);
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
        this.isCast = (id, key, type) => {
            let ref = this.poolIds.get(id);
            if (ref !== undefined && ref != null && ref != void 0) {
                var v = ref.value[key];
                if (v === undefined || v === null || v === void 0)
                    return type.IsClass;
                if (isCSharpType(v))
                    return type.IsAssignableFrom(v.GetType());
                if (isValueType(v)) {
                    var t = typeof (v);
                    return t === 'number' && isNumber(type)
                        || t === 'boolean' && type.Equals(puerts_1.$typeof(CS.System.Boolean))
                        || t === 'string' && type.Equals(puerts_1.$typeof(CS.System.String));
                }
                if (typeof (v) == 'function') {
                    return puerts_1.$typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
                }
                var j_type = puerts_1.$typeof(CS.Puerts.JsBase);
                while (type != null) {
                    if (type === j_type)
                        return true;
                    type = type.BaseType;
                }
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
            if (id === undefined || id === null || id === void 0)
                throw new Error('invalid parameters id');
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
        this.isCastInPath = (id, path, type) => {
            if (id === undefined || id === null || id === void 0)
                throw new Error('invalid parameters id');
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
                var v = tmp[last];
                if (v === undefined || v === null || v === void 0)
                    return type.IsClass;
                if (isCSharpType(v))
                    return type.IsAssignableFrom(v.GetType());
                if (isValueType(v)) {
                    var t = typeof (v);
                    return t === 'number' && isNumber(type)
                        || t === 'boolean' && type.Equals(puerts_1.$typeof(CS.System.Boolean))
                        || t === 'string' && type.Equals(puerts_1.$typeof(CS.System.String));
                }
                if (typeof (v) == 'function') {
                    return puerts_1.$typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
                }
                var j_type = puerts_1.$typeof(CS.Puerts.JsBase);
                while (type != null) {
                    if (type === j_type)
                        return true;
                    type = type.BaseType;
                }
            }
            return false;
        };
        this.contains = (id, path) => {
            if (id === undefined || id === null || id === void 0)
                throw new Error('invalid parameters id');
            var t = typeof (path);
            if (t !== 'number' && t != 'string')
                throw new Error('invalid parameters key');
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