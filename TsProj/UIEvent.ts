import {UnityEngine} from 'csharp'
import {$typeof} from 'puerts'

function init(monoBehaviour: UnityEngine.MonoBehaviour): void {
    let button  = monoBehaviour.GetComponent($typeof(UnityEngine.UI.Button)) as UnityEngine.UI.Button;
    let input = monoBehaviour.transform.parent.GetComponentInChildren($typeof(UnityEngine.UI.InputField)) as UnityEngine.UI.InputField;
    button.onClick.AddListener(() =>{
        console.log("button pressed..., input is: " + input.text);
    });
}

export {init};
