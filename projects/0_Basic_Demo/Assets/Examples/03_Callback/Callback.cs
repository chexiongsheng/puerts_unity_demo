using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public class Callback : MonoBehaviour
    {
        JsEnv jsEnv;

        // Use this for initialization
        void Start()
        {
            //jsEnv = new JsEnv(new DefaultLoader(), -1, BackendType.QuickJS, System.IntPtr.Zero, System.IntPtr.Zero);
            jsEnv = new JsEnv();

            //要使用值类型参数或者返回值的委托要声明，而Callback1因为是引用类型所以不用。
            jsEnv.UsingAction<int>();

            jsEnv.Eval(@"
                let obj = new CS.PuertsTest.TestClass();
                //如果你后续要remove，需要这样构建一个Delegate，后续可以用该Delegate引用去remove
                let delegate = new CS.PuertsTest.Callback1(o => o.Foo()); 
                obj.AddEventCallback1(delegate);
                obj.AddEventCallback2(i => console.log(i)); //如果不需要remove，直接传函数即可
                obj.AddECB((a, b, c) => {
                    console.log(a, b, c);
                    console.log('args', b.get_Item(0), b.get_Item(1));
                });
                obj.Trigger();
                obj.RemoveEventCallback1(delegate);
                obj.Trigger();
                for(var gn in globalThis) console.log(gn);
                globalThis.gc();
            ");
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
}
