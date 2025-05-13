using UnityEngine;
using Puerts;
using System.Collections.Generic;

namespace PuertsTest
{
    public delegate void Callback1(TestClass obj);

    public delegate void Callback2(int str);

    public delegate void EventCallBack(object thisobj, params object[] args);

    public class CMTree<T>
    {
        public class Node
        {
            public Node Parent;
            public List<Node> Child = new List<Node>();

            public T Value;

            public int ChildCount => Child.Count;

            public Node this[int i] => (i < 0 || i >= Child.Count) ? null : Child[i];
        }
    }

    public class TestClass
    {
        Callback1 callback1;

        Callback2 callback2;

        EventCallBack ecb;

        public void AddEventCallback1(Callback1 callback1)
        {
            this.callback1 += callback1;
        }

        public void AddECB(EventCallBack t)
        {
            this.ecb += t;
        }

        public void ABCD(Dictionary<string, System.Collections.Generic.List<int>>.Enumerator e)
        {

        }

        public void SetCanvasSortingOrder(CMTree<GameObject>.Node rootNode, int order)
        {

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
            if (ecb != null)
            {
                ecb(this, 1, "aaa");
            }
            Debug.Log("end Trigger");
        }

        public void Foo()
        {
            Debug.Log("Foo");
        }
    }
}
