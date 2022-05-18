const CS = puertsRequire('csharp');

require('ts-node').register({ project: `${__dirname}/tsconfig.json` });

exports.compileTS = function (tsConfigPath) {
    require('./run-ts.ts').default(tsConfigPath);
}

exports.watch = function (tsConfigPath) {
    const Watcher = require('./watch.ts').default;
    const HotReloadWatcher = new Watcher(tsConfigPath);

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