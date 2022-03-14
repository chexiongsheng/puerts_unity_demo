using UnityEngine;
using Puerts;

namespace PuertsTest
{
    public delegate void Callback1(TestClass obj);

    public delegate void Callback2(int str);

    public class TestClass
    {
        Callback1 callback1;

        Callback2 callback2;

        public void AddEventCallback1(Callback1 callback1)
        {
            this.callback1 += callback1;
        }

        public void RemoveEventCallback1(Callback1 callback1)
        {
            this.callback1 -= callback1;
        }

        public void AddEventCallback2(Callback2 callback2)
        {
            this.callback2 += callback2;
        }

        public void Trigger()
        {
            Debug.Log("begin Trigger");
            if (callback1 != null)
            {
                callback1(this);
            }
            if (callback2 != null)
            {
                callback2(1024);
            }
            Debug.Log("end Trigger");
        }

        public void Foo()
        {
            Debug.Log("Foo");
        }
    }
}
