using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public class TypedValue : MonoBehaviour
    {
        JsEnv jsEnv;

        // Use this for initialization
        void Start()
        {
            jsEnv = new JsEnv();

            jsEnv.Eval(@"
                let value = new CS.Puerts.Int64Value(512n);
                CS.PuertsTest.TypedValue.CallSomeFunction(value);
            ");
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }

        public static void CallSomeFunction(object o) {
            UnityEngine.Debug.Log("value type:" + o.GetType());
        }
    }
}
