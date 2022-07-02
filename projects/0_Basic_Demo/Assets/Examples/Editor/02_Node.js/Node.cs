using UnityEngine;
using Puerts;
using System;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;

namespace PuertsEditorTest
{
    [InitializeOnLoad]
    class Node
    {
        static Node()
        {
            EditorApplication.update += OnUpdate;
        }

        private static JsEnv env;
        public static void OnUpdate() 
        {
            if (env != null) 
            {
                env.Tick();
            }
        }

        [MenuItem("PuertsEditorTest/NodeJS", false, 1)]
        public static async void DoTest()
        {
            if (PuertsDLL.GetLibBackend() == 1) 
            {
                env = new JsEnv(new DefaultLoader(), 9222);
                env.Eval(@"
                    console.log(require('os').cpus().length); 
                    require('fs').readFile('" + Application.dataPath + @"/Examples/09_Node.js/Node.cs', (err, res)=> { console.log(res.toString('utf-8')) });
                    setInterval(()=> onInterval() , 500)
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
                ");

                var task =  new Task(()=> {
                    Thread.Sleep(2000);
                });
                task.Start();
                await task;
                
                env.Dispose();
                env = null;
                
            } else {
                UnityEngine.Debug.LogError("NodeBackend is not supported");
            }
        }
    }
}