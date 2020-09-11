using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public class JsObjectTest
    {
        public GenericDelegate Getter;

        public GenericDelegate Setter;

        public void SetSomeData()
        {
            Setter.Action("a", 1);
            Setter.Action("b.a", 1.1);
        }

        public void GetSomeData()
        {
            Debug.Log(Getter.Func<string, int>("a"));
            Debug.Log(Getter.Func<string, double>("b.a"));
            Debug.Log(Getter.Func<string, int>("c"));
        }
    }

    public class JsObjectAccess : MonoBehaviour
    {
        void Start()
        {
            var jsEnv = new JsEnv();

            jsEnv.Eval(@"
                const CS = require('csharp');
                let obj = new CS.PuertsTest.JsObjectTest();
                let jsObj = {'c': 100};
                obj.Setter = (path, value) => {
                    let tmp = jsObj;
                    let nodes = path.split('.');
                    let lastNode = nodes.pop();
                    nodes.forEach(n => {
                        if (typeof tmp[n] === 'undefined') tmp[n] = {};
                        tmp = tmp[n];
                    });
                    tmp[lastNode] = value;
                }

                obj.Getter = (path) => {
                    let tmp = jsObj;
                    let nodes = path.split('.');
                    let lastNode = nodes.pop();
                    nodes.forEach(n => {
                        if (typeof tmp != 'undefined') tmp = tmp[n];
                    });
                    return tmp[lastNode];
                }
                obj.SetSomeData();
                obj.GetSomeData();
                console.log(JSON.stringify(jsObj));
            ");
            jsEnv.Dispose();
        }
    }
}