using System;
using System.Collections.Generic;

namespace Puerts
{
    public class JsBase : IDisposable
    {
        public string id { get; private set; }
        public JsEnv JsEnv { get; }
        public string Value { get { return RefUtils.Value(JsEnv, id); } }
        public JsBase(string id, JsEnv env)
        {
            this.JsEnv = env;
            this.id = id;
            if (!string.IsNullOrEmpty(id))
                RefUtils.Ref(this.JsEnv, id);
        }
        ~JsBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(this.id) && RefUtils.IsAlive(this.JsEnv))
                RefUtils.Release(this.JsEnv, this.id);
            this.id = null;
        }
    }
}