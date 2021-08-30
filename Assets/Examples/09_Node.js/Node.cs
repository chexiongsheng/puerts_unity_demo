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
            env = new JsEnv(JsEnvMode.Node);
            env.Eval(
                "console.log(require('os').cpus().length); " + 
                "require('fs').readFile('/Users/zombieyang/Documents/build.sh', (err, res)=> { console.log(res.toString('utf-8')) })"
            );
            
        } else {
            UnityEngine.Debug.Log("NodeBackend is not supported");
        }
    }

    // Update is called once per frame
    void Update()
    {
        env.Tick();
    }
}
