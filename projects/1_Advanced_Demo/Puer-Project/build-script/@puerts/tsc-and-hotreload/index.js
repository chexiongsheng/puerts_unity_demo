const { join } = require('path');
const CS = puertsRequire('csharp');

require('ts-node').register({ project: `${__dirname}/tsconfig.json` });

exports.compileTS = function (Application_dataPath) {
    require('./compile.ts').default(join(Application_dataPath, '../Puer-Project/tsconfig.json'));
}

exports.watch = function (Application_dataPath) {
    const Watcher = require('./watch.ts').default;
    const HotReloadWatcher = new Watcher(join(Application_dataPath, '../Puer-Project/tsconfig.json'));

    const jsEnvs = CS.Puerts.JsEnv.jsEnvs
    console.log('jsEnvs.Count:' + jsEnvs.Count);
    for (let i = 0; i < jsEnvs.Count; i++)
    {
        const item = jsEnvs.get_Item(i);
        if (item && item.debugPort != -1) {
            HotReloadWatcher.addDebugger(item.debugPort)
        }
    }

    CS.NodeTSCAndHotReload.addDebugger = HotReloadWatcher.addDebugger.bind(HotReloadWatcher);
    CS.NodeTSCAndHotReload.removeDebugger = HotReloadWatcher.removeDebugger.bind(HotReloadWatcher);
}