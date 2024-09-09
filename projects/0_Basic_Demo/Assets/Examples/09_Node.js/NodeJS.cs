using UnityEngine;
using Puerts;
using System;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PuertsTest
{

    public class NodeJS : MonoBehaviour
    {
        JsEnv env;
        // Start is called before the first frame update
        void Start()
        {
            env = new JsEnv(new DefaultLoader(), 9222);
            if (env.Backend is BackendNodeJS)
            {
                env.ExecuteModule("node-example.mjs");
            }
            else
            {
                UnityEngine.Debug.LogError("NodeBackend is not supported");
                env.Dispose();
                env = null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (env != null)
            {
                env.Tick();
            }
        }

        void OnDestroy()
        {
            if (env != null)
            {
                env.Dispose();
                env = null;
            }
        }
    }
}