class Rotate {
    constructor(bindTo) {
        this.speed = 0;
        this.bindTo = bindTo;
        this.bindTo.JsUpdate = () => this.onUpdate();
        this.bindTo.JsOnDestroy = () => this.onDestroy();
        const propsComponent = this.bindTo.GetComponent(puerts.$typeof(CS.TSProperties));
        for (let i = 0; i < propsComponent.Pairs.Length; i++) {
            const p = propsComponent.Pairs.get_Item(i);
            //@ts-ignore
            this[p.key] = p.value;
        }
        setInterval(() => {
            // 开启hot-reload时，你可以尝试修改该函数感受效果
            this.speed = 3000;
            console.log('current speed is ' + this.speed);
        }, 1000);
    }
    onUpdate() {
        //js不支持操作符重载所以Vector3的乘这么用
        let r = CS.UnityEngine.Vector3.op_Multiply(CS.UnityEngine.Vector3.up, CS.UnityEngine.Time.deltaTime * this.speed);
        this.bindTo.transform.Rotate(r);
    }
    onDestroy() {
        console.log('onDestroy...');
    }
}
export function init(bindTo) {
    new Rotate(bindTo);
}
