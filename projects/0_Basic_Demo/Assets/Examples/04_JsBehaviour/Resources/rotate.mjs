const speed = 10;

class Rotate {
    constructor(bindTo) {
        this.bindTo = bindTo;
        this.bindTo.JsUpdate = () => this.onUpdate();
        this.bindTo.JsOnDestroy = () => this.onDestroy();
        bindTo.StartCoroutine(bindTo.Coroutine());
    }
    
    onUpdate() {
        //js不支持操作符重载所以Vector3的乘这么用
        let r = CS.UnityEngine.Vector3.op_Multiply(CS.UnityEngine.Vector3.up, CS.UnityEngine.Time.deltaTime * speed*100);
        this.bindTo.transform.Rotate(r);
    }
    
    onDestroy() {
        console.log('onDestroy...');
    }
}

export function init(bindTo) {
    new Rotate(bindTo);
}
