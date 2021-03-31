using System;
using System.Collections;
using System.Collections.Generic;
using Puerts;
using UnityEngine;

public class JsObjectAccess1 : MonoBehaviour
{
    private JsEnv jsEnv;
    void Awake()
    {
        jsEnv = new JsEnv();

        CallJs();
        JsCallCSharp();
        CallObjFunc();
    }
    void OnDestroy()
    {
        jsEnv?.Dispose();
    }

    void CallJs()
    {
        Debug.Log("=============CallJs================================");

        var script = @"return { 
            obj: { 
                a: 10, 
                b: { 
                    c: 20 
                } 
            }
        };";
        var jsObject = jsEnv.Load<JsObject>(script);
        var obj = jsObject.Get<JsObject>("obj");
        var b = obj.Get<JsObject>("b");

        Debug.Log(jsObject);
        Debug.Log("obj.a= " + obj.Get<int>("a"));
        Debug.Log("obj.b.c= " + b.Get<int>("c"));

        b.Set("c", 100);
        Debug.Log("obj.b.c= " + b.Get<int>("c"));
        Debug.Log("obj.b.c= " + obj.GetInPath<int>("b.c"));

        Debug.Log("====================================================");
    }
    void JsCallCSharp()
    {
        Debug.Log("==============JsCallCSharp=============================");

        var script = @"return { obj: { a: 10, b: { c: 20 } }};";
        var jsObject = jsEnv.Load<JsObject>(script);
        script = @"
return function(jsObject){
    console.log(jsObject);
    console.log(jsObject.Value);
    var v = eval(jsObject.Value);
    console.log(v);
    console.log(JSON.stringify(v));
};
        ";
        jsEnv.Load(script, jsObject);

        Debug.Log("====================================================");
    }
    void CallObjFunc()
    {
        Debug.Log("=============CallObjFunc===========================");

        var script = @"return { 
            a: () => { return 'call a' }, 
            b: () => { console.log('call b') }, 
            c1: 100, 
            c: function () { console.log(this !== undefined ? this.c1 : 'undefined'); } 
        };";
        var jsObject = jsEnv.Load<JsObject>(script);

        var result = jsObject.Get<Func<string>>("a")();
        Debug.Log(result);
        jsObject.Get<Action>("b")();
        jsObject.Get<Action>("c")();
        jsObject.Call("c");

        Debug.Log("====================================================");
    }
}
