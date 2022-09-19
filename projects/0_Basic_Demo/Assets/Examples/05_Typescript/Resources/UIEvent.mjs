function init(monoBehaviour) {
    let button = monoBehaviour.GetComponent(puerts.$typeof(CS.UnityEngine.UI.Button));
    let input = monoBehaviour.transform.parent.GetComponentInChildren(puerts.$typeof(CS.UnityEngine.UI.InputField));
    let toggle = monoBehaviour.transform.parent.GetComponentInChildren(puerts.$typeof(CS.UnityEngine.UI.Toggle));
    button.onClick.AddListener(() => {
        console.log("button pressed..., input is: " + input.text);
    });
    toggle.onValueChanged.AddListener((b) => {
        console.log("toggle.value=" + b);
    });
}
export { init };
//# sourceMappingURL=UIEvent.js.map