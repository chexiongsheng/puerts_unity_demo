using UnityEngine;
using Puerts;

//typescript工程在TsProj，在该目录执行npm run build即可编译并把js文件拷贝进unity工程

namespace PuertsTest
{
    public class TsQuickStart : MonoBehaviour
    {
        JsEnv jsEnv;

        void Start()
        {
            jsEnv = new JsEnv();
            jsEnv.Eval("require('QuickStart')");
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
}
