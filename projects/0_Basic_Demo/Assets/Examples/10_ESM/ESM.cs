using System;
using UnityEngine;
using Puerts;

public class ESM : MonoBehaviour
{
    JsEnv env;
    // Start is called before the first frame update
    void Start()
    {
        env = new JsEnv();
        Func<JSObject> func = env.ExecuteModule<Func<JSObject>>("module.mjs", "startup");

        UnityEngine.Debug.Log(func());
    }

    // Update is called once per frame
    void Update()
    {
        if (env != null) {
            env.Tick();
        } 
    }

    void OnDestroy()
    {
        env.Dispose();
        env = null;
    }
}
