using System;
using UnityEngine;
using Puerts;

public class UIEvent : MonoBehaviour
{
    static JsEnv jsEnv;

    void Start()
    {
        if (jsEnv == null)
        {
            jsEnv = new JsEnv();
            jsEnv.UsingAction<bool>();//toggle.onValueChanged用到
        }

        var init = jsEnv.Eval<Action<MonoBehaviour>>("const m = require('UIEvent'); m.init;");

        if (init != null) init(this);
    }
}
