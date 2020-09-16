using System;
using System.Collections.Generic;

namespace Puerts
{
    public class JsBase : IDisposable
    {
        public int refId { get; private set; }
        public JsEnv JsEnv { get; }
        public string Value
        {
            get
            {
                return RefUtils.Value(refId);
            }
        }
        public JsBase(int ref_id, JsEnv env)
        {
            this.JsEnv = env;
            this.refId = ref_id;
            if (ref_id >= 0)
                RefUtils.Ref(this.JsEnv, ref_id);
        }
        ~JsBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (this.refId >= 0 && RefUtils.IsAlive(JsEnv))
                RefUtils.Release(this.JsEnv, refId);
            this.refId = -1;
        }
    }
}