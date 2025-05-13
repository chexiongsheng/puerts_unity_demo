using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public class JsCallCs : MonoBehaviour
    {
        JsEnv jsEnv;

        void Start()
        {
            jsEnv = new JsEnv();

            jsEnv.Eval(@"
                let gameObject = new CS.UnityEngine.GameObject('testObject');
                CS.UnityEngine.Debug.Log(gameObject.name);
            ");
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
}
