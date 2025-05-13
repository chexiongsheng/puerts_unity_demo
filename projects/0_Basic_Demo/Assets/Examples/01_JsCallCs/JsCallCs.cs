using UnityEngine;
using Puerts;
using System.Linq;
using System;

public class MyObject
{
    public bool bIsAlive = true;

    private static int sid = 0;

    public int id = sid++;

    private static bool CompareObjects(MyObject lhs, MyObject rhs)
    {
        bool lhsNull = ((object)lhs) == null;
        bool rhsNull = ((object)rhs) == null;

        if (rhsNull && lhsNull) return true;

        if (rhsNull) return !lhs.bIsAlive;
        if (lhsNull) return !rhs.bIsAlive;

        return lhs.id == rhs.id;
    }

    public static bool operator ==(MyObject x, MyObject y)
    {
        return CompareObjects(x, y);
    }

    public static bool operator !=(MyObject x, MyObject y)
    {
        return CompareObjects(x, y);
    }
}

namespace PuertsTest
{
    public class GLoader
    {
        public string url
        {
            get { return _url; }
        }
        string _url = "iiiiiiuu";
    }


    public enum E
    {
        A = 1, B = 2
    }
    
    public class ABCD
    {
        public static void Foo()
        {
            Debug.Log("ABCD.Foo()");
        }

        public void Foo(E val = (E)(-1)) { }
        public void Bar(E val = E.A | E.B) { }

        public static string GetStringContainNull()
        {
            return "\0\0x61\0x73\0x6d";
        }
    }
    public class JsCallCs : MonoBehaviour
    {
        JsEnv jsEnv;

        void Start()
        {
            jsEnv = new JsEnv();
            UnityEngine.Debug.Log("=============================================");
            jsEnv.Eval(@"
                let gameObject = new CS.UnityEngine.GameObject('testObject');
                CS.UnityEngine.Debug.Log(gameObject.name);
                //CS.UnityEngine.Debug.Log(JSON.stringify(v8.getHeapStatistics(), null, 4));
                //CS.UnityEngine.Debug.Log(JSON.stringify(v8.getHeapSpaceStatistics(), null, 4));
                let str = CS.PuertsTest.ABCD.GetStringContainNull();
                console.log(str.length);
                //CS.PuertsTest.ABCD.Foo();
    /*var a = new Map();
    for (var i = 0; i < 1000; i++) {
        a.set(i, i);
    }
    //function assert(l,r) {if (l != r) throw new Error('not eq');}
    function assert(l,r) {console.log(l, r);}
    a.set(-2147483648, 1);
    assert(a.get(-2147483648), 1);
    assert(a.get(-2147483647 - 1), 1);
    assert(a.get(-2147483647.5 - 0.5), 1);

    a.set(1n, 1n);
    assert(a.get(1n), 1n);
    assert(a.get(2n**1000n - (2n**1000n - 1n)), 1n);*/
    class TinyFairyGUIGLoader extends CS.PuertsTest.GLoader {
        foo() {
            const url = super.url;
            console.log(url);
        }
        get url() {
            return '-------------';
        }
    }
    let tfgl = new TinyFairyGUIGLoader();
    tfgl.foo();

console.log('connecting...............');
/*console.log(WebSocket);
let con = new WebSocket('ws://localhost:8080');

con.addEventListener('open', (ev) => {
    console.log(`on open`);
    con.send('puerts websocket');
});

con.addEventListener('message', (ev) => {
    console.log(`on message: ${ev.data}`);
});
con.addEventListener('error', (ev) => {
    console.log(`on error`);
});
con.addEventListener('close', (ev) => {
    console.log(`on close`);
});
setInterval(() => console.log('123'), 1000);*/
");
            //foreach(var method in Puerts.Utils.GetAllTypes().SelectMany(t => t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            /*System.Collections.Generic.List<string> entryPoints = new System.Collections.Generic.List<string>();
            foreach (var method in typeof(Puerts.PuertsDLL).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(m => m.IsDefined(typeof(System.Runtime.InteropServices.DllImportAttribute), false)))
            {
                string entryPoint = method.Name;
                foreach(var attr in method.GetCustomAttributes(false))
                {
                    if (attr is System.Runtime.InteropServices.DllImportAttribute)
                    {
                        var dllimport = (System.Runtime.InteropServices.DllImportAttribute)attr;
                        if (!string.IsNullOrEmpty(dllimport.EntryPoint))
                        {
                            entryPoint = dllimport.EntryPoint;
                        }
                    }
                }
                entryPoints.Add(entryPoint);
            }
            Debug.Log(string.Join('\n', entryPoints.Select(s => s + ";").ToArray()));*/
            /*string str = "\0\0x61\0x73\0x6d";
            Debug.Log($"str.Length={str.Length}");
            int i = 0;
            foreach(var c in str.ToCharArray())
            {
                Debug.Log($"{i++} : {(int)c}");
            }
            */

            MyObject obj1 = new MyObject();
            Debug.Log("obj1 is null:" + (obj1 == null));
            obj1.bIsAlive = false;
            Debug.Log("obj1 is null:" + (obj1 == null));
            MyObject obj2 = new MyObject();
            obj2.id = obj1.id;
            Debug.Log("obj2 is null:" + (obj2 == null));
            Debug.Log("obj1 == obj2:" + (obj1 == obj2));

        }

        public void foo(string a)
        {
            Debug.Log($"foo:{a}");
            //System.Array.CreateInstance()
        }

        private void Update()
        {
            jsEnv.Tick();
        }


        void OnDestroy()
        {
            jsEnv.Dispose();
        }
    }
}
