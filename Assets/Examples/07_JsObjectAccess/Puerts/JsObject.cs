using System;
using System.Collections;
using System.Collections.Generic;

namespace Puerts
{
    /// <summary>
    /// 通过引用实现, 实际使用时将跨语言调用
    /// </summary>
    public class JsObject : JsBase
    {
        public JsObject(string id, JsEnv env) : base(id, env)
        {
        }

        public void Get<TKey, TValue>(TKey key, out TValue value)
        {
            value = RefUtils.Get<TKey, TValue>(JsEnv, id, key);
        }
        public TValue Get<TValue>(string key)
        {
            TValue ret;
            Get(key, out ret);
            return ret;
        }
        public void Set<TKey, TValue>(TKey key, TValue value)
        {
            RefUtils.Set<TKey, TValue>(JsEnv, id, key, value);
        }
        public TResult GetInPath<TResult>(string path)
        {
            return RefUtils.GetInPath<TResult>(JsEnv, id, path); ;
        }
        public void SetInPath<TValue>(string path, TValue val)
        {
            RefUtils.SetInPath<TValue>(JsEnv, id, path, val); ;
        }
        public bool IsCast(object key, Type type)
        {
            return RefUtils.IsCast(JsEnv, id, key, type);
        }
        public bool IsCastInPath(string path, Type type)
        {
            return RefUtils.IsCastInPath(JsEnv, id, path, type);
        }
        public bool Contains(string path)
        {
            return RefUtils.Contains(JsEnv, id, path);
        }

        public T Cast<T>()
        {
            object v = RefUtils.Load<object>(JsEnv, "eval(" + Value + ")");
            if (v != null && typeof(T).IsAssignableFrom(v.GetType()))
                return (T)v;
            return default;
        }

        public string[] GetKeys()
        {
            return RefUtils.Keys(JsEnv, id);
        }
        public void ForEach<TValue>(Action<string, TValue> action)
        {
            RefUtils.ForEach<TValue>(JsEnv, id, action);
        }
        public int Length { get { return RefUtils.Length(JsEnv, id); } }

        public void Call(string name, params object[] args)
        {
            RefUtils.Call(JsEnv, id, name, args);
        }
        public T Call<T>(string name, params object[] args)
        {
            return RefUtils.Call<T>(JsEnv, id, name, args);
        }
    }
}