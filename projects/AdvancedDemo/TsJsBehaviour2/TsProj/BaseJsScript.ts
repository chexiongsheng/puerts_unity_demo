import { PuertsTest, UnityEngine } from "csharp";
import { $typeof } from "puerts";

export interface IBaseJsScript
{
    name?: string;
	dontDestroyOnLoad?: boolean;
    Start?: Function;
    FixedUpdate?: Function;
    Update?: Function;
    LateUpdate?: Function;
    OnDestroy?: Function;
}

export class BaseJsScript
{
    public scriptObject: PuertsTest.JsBehaviour2;
    public scriptArg: IBaseJsScript;
    constructor( scriptArg: IBaseJsScript ) {
        this.scriptArg = scriptArg;
        this.scriptObject = new UnityEngine.GameObject().AddComponent( $typeof(PuertsTest.JsBehaviour2) ) as PuertsTest.JsBehaviour2;
        if( this.scriptArg.name ) this.scriptObject.name = this.scriptArg.name;
		if( this.scriptArg.dontDestroyOnLoad ) UnityEngine.GameObject.DontDestroyOnLoad( this.scriptObject );

        if( typeof(this.scriptArg.Start) === 'function' ) 
            this.scriptObject.JsStart = () => this.scriptArg.Start();

        if( typeof(this.scriptArg.FixedUpdate) === 'function' ) 
            this.scriptObject.JsFixedUpdate = () => this.scriptArg.FixedUpdate();

        if( typeof(this.scriptArg.Update) === 'function' ) 
            this.scriptObject.JsUpdate = () => this.scriptArg.Update();

        if( typeof(this.scriptArg.LateUpdate) === 'function' ) 
            this.scriptObject.JsLateUpdate = () => this.scriptArg.LateUpdate();

        if( typeof(this.scriptArg.OnDestroy) === 'function' ) 
            this.scriptObject.JsOnDestroy = () => this.scriptArg.OnDestroy();
    }
    /*Start() : void {
        UnityEngine.Debug.Log( "BaseJsScript.Start()" );
    }
    FixedUpdate() : void {    
        UnityEngine.Debug.Log( "BaseJsScript.FixedUpdate()" );  
    }
    JsUpdate() : void {    
        UnityEngine.Debug.Log( "BaseJsScript.Update()" );  
    }
    LateUpdate() : void {    
        UnityEngine.Debug.Log( "BaseJsScript.LateUpdate()" );  
    }
    OnDestroy() : void {    
        UnityEngine.Debug.Log( "BaseJsScript.OnDestroy()" );  
    }*/
}
