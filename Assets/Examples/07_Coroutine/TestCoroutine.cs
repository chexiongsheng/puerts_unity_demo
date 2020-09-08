using System.Collections;
using System.Collections.Generic;
using Puerts;
using UnityEngine;

namespace PuertsTest
{
    public class TestCoroutine : MonoBehaviour
    {
        public delegate void Init(TestCoroutine monoBehaviour);

        static JsEnv jsEnv;


        void Awake()
        {
            if (jsEnv == null)
            {
                jsEnv = new JsEnv();
                jsEnv.UsingFunc<bool>();
            }

            var init = jsEnv.Eval<Init>("const m = require('coroutine'); m.init;");
            if (init != null)
                init(this);
        }

        void OnDestroy()
        {
            jsEnv?.Dispose();
        }
    }
}
