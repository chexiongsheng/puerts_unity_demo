
function init(monoBehaviour: CS.UnityEngine.MonoBehaviour): void {
    let button  = monoBehaviour.GetComponent(puer.$typeof(CS.UnityEngine.UI.Button)) as CS.UnityEngine.UI.Button;
    let input = monoBehaviour.transform.parent.GetComponentInChildren(puer.$typeof(CS.UnityEngine.UI.InputField)) as CS.UnityEngine.UI.InputField;
    let toggle = monoBehaviour.transform.parent.GetComponentInChildren(puer.$typeof(CS.UnityEngine.UI.Toggle)) as CS.UnityEngine.UI.Toggle;
    button.onClick.AddListener(() =>{
        console.log("button pressed..., input is: " + input.text);
    });
    toggle.onValueChanged.AddListener((b) => {
        console.log("toggle.value=" + b);
    });
}

export {init};
