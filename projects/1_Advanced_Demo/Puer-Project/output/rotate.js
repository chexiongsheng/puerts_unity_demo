"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const csharp_1 = __importDefault(require("csharp"));
const puerts = require('puerts');
class Rotate {
    constructor(bindTo) {
        this.speed = 0;
        this.bindTo = bindTo;
        this.bindTo.JsUpdate = () => this.onUpdate();
        this.bindTo.JsOnDestroy = () => this.onDestroy();
        const propsComponent = this.bindTo.GetComponent(puerts.$typeof(csharp_1.default.TSProperties));
        for (let i = 0; i < propsComponent.Pairs.Length; i++) {
            const p = propsComponent.Pairs.get_Item(i);
            //@ts-ignore
            this[p.key] = p.value;
        }
    }
    onUpdate() {
        //js不支持操作符重载所以Vector3的乘这么用
        let r = csharp_1.default.UnityEngine.Vector3.op_Multiply(csharp_1.default.UnityEngine.Vector3.up, csharp_1.default.UnityEngine.Time.deltaTime * this.speed);
        this.bindTo.transform.Rotate(r);
    }
    onDestroy() {
        console.log('onDestroy...');
    }
}
exports.init = function (bindTo) {
    new Rotate(bindTo);
};
