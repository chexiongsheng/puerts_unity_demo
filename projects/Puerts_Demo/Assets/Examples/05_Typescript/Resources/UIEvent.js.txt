"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.init = void 0;
var csharp_1 = require("csharp");
var puerts_1 = require("puerts");
function init(monoBehaviour) {
    let button = monoBehaviour.GetComponent((0, puerts_1.$typeof)(csharp_1.UnityEngine.UI.Button));
    let input = monoBehaviour.transform.parent.GetComponentInChildren((0, puerts_1.$typeof)(csharp_1.UnityEngine.UI.InputField));
    let toggle = monoBehaviour.transform.parent.GetComponentInChildren((0, puerts_1.$typeof)(csharp_1.UnityEngine.UI.Toggle));
    button.onClick.AddListener(() => {
        console.log("button pressed..., input is: " + input.text);
    });
    toggle.onValueChanged.AddListener((b) => {
        console.log("toggle.value=" + b);
    });
}
exports.init = init;
//# sourceMappingURL=UIEvent.js.map