using System;
using System.Collections;
using System.Collections.Generic;

namespace Puerts
{
    public class JsObject : JsBase
    {
        public JsObject(int reference, JsEnv env) : base(reference, env)
        {
        }

        public void Get<TKey, TValue>(TKey key, out TValue value)
        {
            value = RefUtils.Get<TKey, TValue>(JsEnv, refId, key);
        }
        public TValue Get<TValue>(string key)
        {
            TValue ret;
            Get(key, out ret);
            return ret;
        }
        public void Set<TKey, TValue>(TKey key, TValue value)
        {
            RefUtils.Set<TKey, TValue>(JsEnv, refId, key, value);
        }
        public TResult GetInPath<TResult>(string path)
        {
            return RefUtils.GetInPath<TResult>(JsEnv, refId, path); ;
        }
        public void SetInPath<TValue>(string path, TValue val)
        {
            RefUtils.SetInPath<TValue>(JsEnv, refId, path, val); ;
        }
        public bool Contains(string path)
        {
            return RefUtils.Contains(JsEnv, refId, path);
        }

        public T Cast<T>()
        {
            object v = RefUtils.Load<object>(JsEnv, "eval(" + Value + ")");
            if (v != null && typeof(T).IsAssignableFrom(v.GetType()))
                return (T)v;
            return default;
        }

        public IEnumerable GetKeys()
        {
            yield return null;
        }
        public IEnumerable<T> GetKeys<T>()
        {
            yield return default;
        }

        public void ForEach<TKey, TValue>(Action<TKey, TValue> action) { }
        public int Length { get { return 0; } }
    }
}