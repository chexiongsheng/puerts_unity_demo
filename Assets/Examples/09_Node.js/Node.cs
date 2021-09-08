using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puerts;

public class Node : MonoBehaviour
{
    JsEnv env;
    // Start is called before the first frame update
    void Start()
    {
        if (PuertsDLL.IsJSEngineBackendSupported(JsEnvMode.Node)) 
        {
            env = new JsEnv(JsEnvMode.Node, 9222);
            env.Eval(
                "console.log(require('os').cpus().length); " + 
                "require('fs').readFile('" + Application.dataPath + "/Examples/09_Node.js/Node.cs', (err, res)=> { console.log(res.toString('utf-8')) })"
            );
            
        } else {
            UnityEngine.Debug.LogError("NodeBackend is not supported");
        }
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
