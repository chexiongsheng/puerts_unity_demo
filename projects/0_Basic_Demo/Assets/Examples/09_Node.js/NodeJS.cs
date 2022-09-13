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
            if (PuertsDLL.GetLibBackend() == 1)
            {
                env = new JsEnv(new DefaultLoader(), 9222);
                env.Eval(@"
                    console.log(require('node:os').cpus().length); 
                    const fs = require('node:fs');
                    const path = require('node:path');
                    const util = require('node:util');

                    const CS_Application = CS.UnityEngine.Application;

                    const itv = setInterval(onInterval, 500)
                    function onInterval() {
                        throw Error('interval error');
                    }
                    setTimeout(()=> {
                        clearInterval(itv);
                    }, 1600)  

                    try {
                        a();
                    } catch(e) {
                        console.log(e.stack);
                    }
                    function a() { b() }
                    function b() { c() }
                    function c() { d() }
                    function d() { e() }
                    function e() { f() }
                    function f() { g() }
                    function g() { h() }
                    function h() { throw new Error('error stack trace test') }
                    ;

                    (async function() {
                        const nodeRequire = require('node:module').createRequire(`${CS_Application.persistentDataPath}/axios-project/`);
                        const yauzl = puerts.require('node-unzip.cjs');

                        console.log('streamingAssets copying');
                        var req = CS.UnityEngine.Networking.UnityWebRequest.Get(
                            (CS_Application.streamingAssetsPath.indexOf('file://') != -1 ? '' : 'file://') + 
                            CS_Application.streamingAssetsPath + '/axios-project.zip'
                        );
                        req.SendWebRequest();
                        while (!req.isDone && !(req.isNetworkError || req.isHttpError)) {}

                        if (req.isNetworkError || req.isHttpError) 
                            throw new Exception('copy failed: ' + req.error);
                        else {
                            console.log(req.downloadHandler.data.Length);
                            CS.System.IO.File.WriteAllBytes(
                                CS_Application.persistentDataPath + '/axios-project.zip',
                                req.downloadHandler.data
                            );
                        }

                        console.log('npm extracting...');
                        zipfile = await util.promisify(yauzl.open)(`${CS_Application.persistentDataPath}/axios-project.zip`, { lazyEntries: true });
                        
                        zipfile.readEntry();
                        zipfile.on('entry', function(entry) {
                            if (/\/$/.test(entry.fileName)) {
                                // Directory file names end with '/'.
                                // Note that entries for directories themselves are optional.
                                // An entry's fileName implicitly requires its parent directories to exist.
                                zipfile.readEntry();
                            } else {
                                // file entry
                                zipfile.openReadStream(entry, function(err, readStream) {
                                    if (err) throw err;
                                    readStream.on('end', function() {
                                        zipfile.readEntry();
                                    });
                                    var targetPath = path.join(CS_Application.persistentDataPath, entry.fileName);
                                    var targetDir = path.dirname(targetPath);
                                    if (!fs.existsSync(targetDir)) {
                                        fs.mkdirSync(targetDir, { recursive: true })
                                    }
                                    readStream.pipe(fs.createWriteStream(targetPath));
                                });
                            }
                        });
                        await new Promise((resolve, reject) => {
                            zipfile.on('end', resolve);
                        });

                        console.log('create server');
                        require('node:http').createServer((req, res)=> {}).listen(8081, ()=> { console.log('listened: ', 8081) });

                        console.log('axios start');
                        const axios = nodeRequire('axios');
                        await axios.get('https://puerts.github.io')
                            .then((result) => console.log('axios: ', result.data));
                    })().catch(e=> console.error(e));
                ");
            }
            else
            {
                UnityEngine.Debug.LogError("NodeBackend is not supported");
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