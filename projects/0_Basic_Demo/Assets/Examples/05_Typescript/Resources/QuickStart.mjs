//部署:npm run build
import './ExtensionDecl.mjs';
const { $ref, $unref, $generic, $promise, $typeof } = puer;
//静态函数
CS.UnityEngine.Debug.Log('hello world');
//对象构造
let obj = new CS.PuertsTest.DerivedClass();
//实例成员访问
obj.BMFunc(); //父类方法
obj.DMFunc(CS.PuertsTest.MyEnum.E1); //子类方法
console.log(obj.BMF, obj.DMF);
obj.BMF = 10; //父类属性
obj.DMF = 30; //子类属性
console.log(obj.BMF, obj.DMF);
//静态成员
console.log(CS.PuertsTest.BaseClass.BSF, CS.PuertsTest.DerivedClass.DSF, CS.PuertsTest.DerivedClass.BSF);
//委托，事件
//如果你后续不需要-=，可以像这样直接传函数当delegate
obj.MyCallback = msg => console.log("do not need remove, msg=" + msg);
let callback = obj.MyCallback;
callback.Invoke("111>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>?????????????");
const target = (value) => {
    return typeof value;
};
const proxy = new Proxy(target, {
    apply(target, thisArg, argArray) {
        return Reflect.apply(target, thisArg, argArray);
    },
});
console.log(proxy("sss"));
const fakeFunc = () => { };
fakeFunc.cb = callback;
const proxy2 = new Proxy(fakeFunc, {
    apply(target, thisArg, argArray) {
        return Reflect.apply(target.cb.Invoke, target.cb, argArray);
    },
});
/*const proxy2 = new Proxy<CS.PuertsTest.MyCallback>(callback, {
    apply(target, thisArg: any, argArray: any[]): any {
        return Reflect.apply(target.Invoke, target, argArray);
    },
});*/
proxy2("222>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
//通过new构建的delegate，后续可以拿这个引用去-=
//let delegate = new CS.PuertsTest.MyCallback(msg => console.log('can be removed, msg=' + msg));
//由于ts不支持操作符重载，Delegate.Combine相当于C#里头的obj.myCallback += delegate;
//obj.MyCallback = CS.System.Delegate.Combine(obj.MyCallback, delegate) as CS.PuertsTest.MyCallback;
//obj.Trigger();
//Delegate.Remove相当于C#里头的obj.myCallback -= delegate;
//obj.MyCallback = CS.System.Delegate.Remove(obj.MyCallback, delegate) as CS.PuertsTest.MyCallback;
//obj.Trigger();
//# sourceMappingURL=QuickStart.mjs.map