using UnityEngine;
using Puerts;
using System;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;

namespace PuertsTest
{

    public class NodeJS : MonoBehaviour
    {
        JsEnv env;
        // Start is called before the first frame update
        void Start()
        {
            if (PuertsDLL.GetLibBackend() == 1)
            {
                env = new JsEnv(new DefaultLoader(), 9222);
                env.Eval(@"
                    console.log(require('os').cpus().length); 
                    require('fs').readFile('" + Application.dataPath + @"/Examples/09_Node.js/Node.cs', (err, res)=> { console.log(res.toString('utf-8')) });
                    const itv = setInterval(onInterval , 500)
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

                    // 只有在编辑器建议这么做，打包之后dataPath位置并不确定
                    // only recommend to do this in Editor.
                    const nodeRequire = require('node:module').createRequire('" + Application.dataPath + @"/../TsProj/');
                    const axios = nodeRequire('axios');

                    setTimeout(()=> {
                        clearInterval(itv);
                        axios.get('https://puerts.github.io')
                            .then((result) => console.log('axios: ', result.data));
                    }, 1600)
                ");
            }
            else
            {
                UnityEngine.Debug.LogError("NodeBackend is not supported");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (env != null)
            {
                env.Tick();
            }
        }

        void OnDestroy()
        {
            if (env != null)
            {
                env.Dispose();
                env = null;
            }
        }
    }
}