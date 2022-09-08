//部署:npm run build

import './ExtensionDecl'

const { $ref, $unref, $generic, $promise, $typeof } = puerts;

//静态函数
CS.UnityEngine.Debug.Log('hello world');

//对象构造
let obj = new CS.PuertsTest.DerivedClass();

//实例成员访问
obj.BMFunc();//父类方法
obj.DMFunc(CS.PuertsTest.MyEnum.E1);//子类方法
console.log(obj.BMF, obj.DMF);
obj.BMF = 10;//父类属性
obj.DMF = 30;//子类属性
console.log(obj.BMF, obj.DMF);

//静态成员
console.log(CS.PuertsTest.BaseClass.BSF, CS.PuertsTest.DerivedClass.DSF, CS.PuertsTest.DerivedClass.BSF);

//委托，事件
//如果你后续不需要-=，可以像这样直接传函数当delegate
obj.MyCallback = msg => console.log("do not need remove, msg=" + msg);
//通过new构建的delegate，后续可以拿这个引用去-=
let delegate = new CS.PuertsTest.MyCallback(msg => console.log('can be removed, msg=' + msg));
//由于ts不支持操作符重载，Delegate.Combine相当于C#里头的obj.myCallback += delegate;
obj.MyCallback = CS.System.Delegate.Combine(obj.MyCallback, delegate) as CS.PuertsTest.MyCallback;
obj.Trigger();
//Delegate.Remove相当于C#里头的obj.myCallback -= delegate;
obj.MyCallback = CS.System.Delegate.Remove(obj.MyCallback, delegate) as CS.PuertsTest.MyCallback;
obj.Trigger();
//事件
obj.add_MyEvent(delegate);
obj.Trigger();
obj.remove_MyEvent(delegate);
obj.Trigger();
//静态事件
CS.PuertsTest.DerivedClass.add_MyStaticEvent(delegate);
obj.Trigger();
CS.PuertsTest.DerivedClass.remove_MyStaticEvent(delegate);
obj.Trigger();

//可变参数
obj.ParamsFunc(1024, 'haha', 'hehe', 'heihei');

//in out 参数
let p1 = $ref(1);
let p2 = $ref(10);
let ret = obj.InOutArgFunc(100, p1, p2);
console.log('ret=' + ret + ', out=' + $unref(p1) + ', ref=' + $unref(p2));

//泛型
//先通过$generic实例化泛型参数
let List = $generic(CS.System.Collections.Generic.List$1, CS.System.Int32);//$generic调用性能不会太好，同样泛型参数建议整个工程，至少一个文件内只做一次
let Dictionary = $generic(CS.System.Collections.Generic.Dictionary$2, CS.System.String, List);

let lst = new List<number>();
lst.Add(1);
lst.Add(0);
lst.Add(2);
lst.Add(4);
obj.PrintList(lst);
let dic = new Dictionary<string, CS.System.Collections.Generic.List$1<number>>();
dic.Add("aaa", lst)
obj.PrintList(dic.get_Item("aaa"));

//arraybuffer
let ab = obj.GetAb(5);
let u8a0 = new Uint8Array(ab);
console.log(obj.SumOfAb(u8a0));
let u8a1 = new Uint8Array(2);
u8a1[0] = 123;
u8a1[1] = 101;
console.log(obj.SumOfAb(u8a1));

//引擎api
let go = new CS.UnityEngine.GameObject("testObject");
go.AddComponent($typeof(CS.UnityEngine.ParticleSystem));
go.transform.position = new CS.UnityEngine.Vector3(7, 8, 9);

//extension methods
obj.PlainExtension();
obj.Extension1();
obj.Extension2(go);
let obj1 = new CS.PuertsTest.BaseClass1();
obj.Extension2(obj1);


//typescript和c#的async，await联动，为了不在低版本的Unity下报错，先注释，c#7.3以上版本可以打开这些注释
/*async function asyncCall() {
    let task = obj.GetFileLength("Assets/Examples/05_Typescript/TsQuickStart.cs");
    let result = await $promise(task);
    console.log('file length is ' + result);
    let task2 = obj.GetFileLength("notexistedfile");//这个会抛文件找不到异常，被catch
    let result2 = await $promise(task2);
    console.log('file length is ,' + result2);
}
asyncCall().catch(e => console.error("catch:" + e));*/
