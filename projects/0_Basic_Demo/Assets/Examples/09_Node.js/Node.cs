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
        if (PuertsDLL.GetLibBackend() == 1) 
        {
            env = new JsEnv(new DefaultLoader(), 9222);
            env.Eval(
                @"console.log(require('os').cpus().length); 
                require('fs').readFile('" + Application.dataPath + @"/Examples/09_Node.js/Node.cs', (err, res)=> { console.log(res.toString('utf-8')) });
                setInterval(()=> onInterval() , 1000)
                function onInterval() {
                    throw Error('interval error');
                }

                try {
                    a();
                } catch(e) {
                    console.log(e.stack);
                }
                function a() { b() }
                function b() { c() }
                function c() { d() }
                function d() { e() }
                function e() { f() }
                function f() { g() }
                function g() { h() }
                function h() { throw new Error('error stack trace test') }
                "
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
