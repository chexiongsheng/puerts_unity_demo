using System;
using System.Collections;
using System.Collections.Generic;
using Puerts;
using UnityEngine;

public class JsObjectTest : MonoBehaviour
{
    private JsEnv jsEnv;
    void Start()
    {
        jsEnv = new JsEnv();

        CSharpCallJs();
        JsCallCSharp();
        CSharpCallAction();
    }

    void CSharpCallJs()
    {
        Debug.Log("=============CSharpCallJs==============================");

        var script = @"return { obj: { a: 10, b: { c: 20 } }};";
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
    var v = eval(jsObject.Value);
    console.log(v);
    console.log(JSON.stringify(v));
};
        ";
        jsEnv.Load(script, jsObject);

        Debug.Log("====================================================");
    }
    void CSharpCallAction()
    {
        Debug.Log("=============CSharpCallAction=======================");

        var script = @"return { a: () => { return 'call a' }, b: () => { console.log('call b') } };";
        var jsObject = jsEnv.Load<JsObject>(script);

        var a = jsObject.Get<Func<string>>("a");
        if (a != null)
        {
            Debug.Log(a());
        }
        var b = jsObject.Get<Action>("b");
        if (b != null)
        {
            b();
        }

        Debug.Log("====================================================");
    }
    void OnDestroy()
    {
        jsEnv?.Dispose();
    }
}
