import { UnityEngine } from "csharp";
import { IBaseJsScript, BaseJsScript } from "./BaseJsScript";

export class MyAnotherScript extends BaseJsScript {
    static scriptName: string = "MyAnotherScript";
    constructor( name?: string ) {
        const scriptArg: IBaseJsScript = {
            name: MyAnotherScript.scriptName + ( name ? name : "" ),
            Start: () => this.Start(),
            Update: () => this.Update(),
            OnDestroy: () => this.OnDestroy()
        };
        super( scriptArg );
    }

    Start() : void {
        UnityEngine.Debug.LogWarning( this.scriptObject.name +".Start()" );
    }

    Update() : void {
        UnityEngine.Debug.Log( this.scriptObject.name +".Update()" );
    }

    OnDestroy() : void {
        UnityEngine.Debug.LogError( this.scriptObject.name +".OnDestroy()" );
    }
}
