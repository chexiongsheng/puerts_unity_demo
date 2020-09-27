using UnityEngine;
using System.Collections.Generic;
#if CSHARP_7_3_OR_NEWER
using System.IO;
using System.Threading.Tasks;
#endif

//typescript工程在TsProj，在该目录执行npm run build即可编译并把js文件拷贝进unity工程

namespace PuertsTest
{
    public enum MyEnum
    {
        E1,
        E2
    }

    public delegate void MyCallback(string msg);

    public class BaseClass
    {
        public static void BSFunc()
        {
            Debug.Log("BaseClass Static Func, BSF = " + BSF);
        }

        public static int BSF = 1;

        public void BMFunc()
        {
            Debug.Log("BaseClass Member Func, BMF = " + BMF);
        }

        public int BMF { get; set; }
    }

    public class DerivedClass : BaseClass
    {
        public static void DSFunc()
        {
            Debug.Log("DerivedClass Static Func, DSF = " + DSF);
        }

        public static int DSF = 2;

        public void DMFunc()
        {
            Debug.Log("DerivedClass Member Func, DMF = " + DMF);
        }

        public MyEnum DMFunc(MyEnum myEnum)
        {
            Debug.Log("DMFunc(MyEnum myEnum), myEnum = " + myEnum);
            return MyEnum.E2;
        }

        public int DMF { get; set; }

        public MyCallback MyCallback;

        public event MyCallback MyEvent;

        public static event MyCallback MyStaticEvent;

        public void Trigger()
        {
            Debug.Log("begin Trigger");
            if (MyCallback != null)
            {
                MyCallback("hello");
            }
            if (MyEvent != null)
            {
                MyEvent("john");
            }
            if (MyStaticEvent != null)
            {
                MyStaticEvent("static event");
            }
            Debug.Log("end Trigger");
        }

        public int ParamsFunc(int a, params string[] b)
        {
            Debug.Log("ParamsFunc.a = " + a);
            for (int i = 0; i < b.Length; i++)
            {
                Debug.Log("ParamsFunc.b[" + i + "] = " + b[i]);
            }
            return a + b.Length;
        }

        public double InOutArgFunc(int a, out int b, ref int c)
        {
            Debug.Log("a=" + a + ",c=" + c);
            b = 100;
            c = c * 2;
            return a + b;
        }

        public void PrintList(List<int> lst)
        {
            Debug.Log("lst.Count=" + lst.Count);
            for (int i = 0; i < lst.Count; i++)
            {
                Debug.Log(string.Format("lst[{0}]={1}", i, lst[i]));
            }
        }

        public Puerts.ArrayBuffer GetAb(int size)
        {
            byte[] bytes = new byte[size];
            for(int i = 0; i < size; i++)
            {
                bytes[i] = (byte)(i + 10);
            }
            return new Puerts.ArrayBuffer(bytes);
        }

        public int SumOfAb(Puerts.ArrayBuffer ab)
        {
            int sum = 0;
            foreach (var b in ab.Bytes)
            {
                sum += b;
            }
            return sum;
        }

#if CSHARP_7_3_OR_NEWER
        public async Task<int> GetFileLength(string path)
        {
            Debug.Log("start read " + path);
            using (StreamReader reader = new StreamReader(path))
            {
                string s = await reader.ReadToEndAsync();
                Debug.Log("read " + path + " completed");
                return s.Length;
            }
        }
#endif
    }

    public class BaseClass1
    {

    }

    public class DerivedClass1 : BaseClass1
    {
    }

    public static class BaseClassExtension
    {
        public static void PlainExtension(this BaseClass a)
        {
            Debug.Log("PlainExtension");
        }

        public static T Extension1<T>(this T a) where T : BaseClass
        {
            Debug.Log(string.Format("Extension1<{0}>", typeof(T)));
            return a;
        }

        public static T Extension2<T>(this T a, GameObject b) where T : BaseClass
        {
            Debug.Log(string.Format("Extension2<{0}>", typeof(T)), b);
            return a;
        }

        public static void Extension2<T1, T2>(this T1 a, T2 b) where T1 : BaseClass where T2 : BaseClass1
        {
            Debug.Log(string.Format("Extension2<{0},{1}>", typeof(T1), typeof(T2)));
        }

        public static T UnsupportedExtension<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>();
        }
    }
}
