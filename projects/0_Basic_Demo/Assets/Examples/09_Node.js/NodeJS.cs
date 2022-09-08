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
                    const result = require('child_process').spawnSync('pwd', {cwd: '" + Application.persistentDataPath + @"'});
                    console.log(
                        JSON.stringify(result), result.status, result.stdout && result.stdout.toString('utf-8'), result.stderr && result.stderr.toString('utf-8')
                    );
                    require('fs').readFile('" + Application.dataPath + @"/Examples/09_Node.js/NodeJS.cs', (err, res)=> { 
                        console.log(res.toString('utf-8')) 
                        throw new Error('any Error in node callback'); 
                    });
                    const itv = setInterval(onInterval, 500)
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

                    setTimeout(()=> {
                        clearInterval(itv);
                        const axios = nodeRequire('axios');
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