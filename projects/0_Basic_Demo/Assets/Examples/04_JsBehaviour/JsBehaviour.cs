using UnityEngine;
using Puerts;
using System;
using System.Collections;

namespace PuertsTest
{
    public delegate void ModuleInit(JsBehaviour monoBehaviour);

    //只是演示纯用js实现MonoBehaviour逻辑的可能，
    //但从性能角度这并不是最佳实践，会导致过多的跨语言调用
    public class JsBehaviour : MonoBehaviour
    {
        public string ModuleName;//可配置加载的js模块

        public Action JsStart;
        public Action JsUpdate;
        public Action JsOnDestroy;

        static JsEnv jsEnv;

        private void OnEnable()
        {
            if (jsEnv == null) jsEnv = new JsEnv(new DefaultLoader(), 9229);
            var varname = "m_" + Time.frameCount;
            var init = jsEnv.ExecuteModule<ModuleInit>(ModuleName, "init");

            if (init != null) init(this);

            Application.runInBackground = true;
        }

        private void OnDisable()
        {
            if (JsOnDestroy != null) JsOnDestroy();
        }

        void Start()
        {
            if (JsStart != null) JsStart();
        }

        void Update()
        {
            jsEnv.Tick();
            if (JsUpdate != null) JsUpdate();
        }

        void OnDestroy()
        {
            JsStart = null;
            JsUpdate = null;
            JsOnDestroy = null;
        }

        public IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(1);
            UnityEngine.Debug.Log("coroutine done");
        }
    }
}