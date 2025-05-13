using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public class Module : MonoBehaviour
    {
        JsEnv jsEnv;

        // Use this for initialization
        void Start()
        {
            //JsEnv还有一个构造函数可以传loader，
            //通过实现不同的loader，可以做到从诸如
            //AssetBundle，压缩包，网络等源加载代码
            //这个无参构造会用默认的loader，默认loader
            //从Resources目录加载
            jsEnv = new JsEnv();

            jsEnv.ExecuteModule("main.mjs");
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
}