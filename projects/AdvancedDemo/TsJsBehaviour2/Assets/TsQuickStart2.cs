using UnityEngine;
using Puerts;
using System;

//typescript工程在TsProj，在该目录执行npm run build即可编译并把js文件拷贝进unity工程

namespace PuertsTest
{
    public class TsQuickStart2 : MonoBehaviour
    {
        JsEnv jsEnv;

        void Start()
        {
            Application.runInBackground = true;
            jsEnv = new JsEnv();
            //jsEnv = new JsEnv(new DefaultLoader(UnityEngine.Application.dataPath + "../TsProj/output/"), 8080);
            //jsEnv.WaitDebugger();
            jsEnv.Eval("require('QuickStart2')");
        }

        private void Update()
        {
            jsEnv.Tick();
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
    public class JsBehaviour2 : MonoBehaviour
    {
        public Action JsStart;
        public Action JsFixedUpdate;
        public Action JsUpdate;
        public Action JsLateUpdate;
        public Action JsOnDestroy;

        void Start()
        {
            JSAction(JsStart);
        }

        void FixedUpdate()
        {
            JSAction(JsFixedUpdate);
        }

        void Update()
        {
            JSAction(JsUpdate);
        }
        void LateUpdate()
        {
            JSAction(JsLateUpdate);
        }

        void JSAction(Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        void OnEnable()
        {
            Start();
        }

        void OnDisable()
        {
            JSAction(JsOnDestroy);
        }

        void OnDestroy()
        {
            JsStart = null;
            JsFixedUpdate = null;
            JsUpdate = null;
            JsLateUpdate = null;
            JsOnDestroy = null;
        }
    }
}