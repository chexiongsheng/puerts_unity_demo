using UnityEngine;
using Puerts;
using System;

//typescript工程在TsProj，在该目录执行npm run build即可编译并把js文件拷贝进unity工程

namespace PuertsTest
{
    public class TsQuickStart : MonoBehaviour
    {
        JsEnv jsEnv;

        void Start()
        {
            jsEnv = new JsEnv();
            //jsEnv = new JsEnv(new DefaultLoader("E:/puerts_unity_demo/TsProj/output/"), 8080);
            //jsEnv.WaitDebugger();
            jsEnv.Eval("require('QuickStart')");
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
}
