import { UnityEngine } from "csharp";
import { IBaseJsScript, BaseJsScript } from "./BaseJsScript";

export class MyScript extends BaseJsScript {
    static scriptName: string = "MyScript";
    constructor() {
        const scriptArg: IBaseJsScript = {
            name: MyScript.scriptName,
            Start: () => this.Start(),
            FixedUpdate: () => this.FixedUpdate(),
            Update: () => this.Update(),
            LateUpdate: () => this.LateUpdate(),
            OnDestroy: () => this.OnDestroy(),
        };
        super( scriptArg );
    }

    Start() : void {
        UnityEngine.Debug.LogWarning( "MyScript.Start()" );
    }

    FixedUpdate() : void {
        UnityEngine.Debug.Log( "MyScript.FixedUpdate()" );
    }

    Update() : void {
        UnityEngine.Debug.Log( "MyScript.Update()" );
    }

    LateUpdate() : void {
        UnityEngine.Debug.Log( "MyScript.LateUpdate()" );
    }
    
    OnDestroy() : void {
        UnityEngine.Debug.LogError( "MyScript.OnDestroy()" );
    }
}