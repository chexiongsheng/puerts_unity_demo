const fs = require('fs');
const path = require('path');
const teaCup = require('./@puer/tsc');
const Watcher = require('./@puer/v8-hotreload').Watcher
const HotReloader = new Watcher();

const jsEnvs = CS.Puerts.JsEnv.jsEnvs
for (let i = 0; i < jsEnvs.Count; i++)
{
    const item = jsEnvs.get_Item(i);
    if (item && item.debugPort != -1) {
        HotReloader.addDebugger(item.debugPort)
    }
}

CS.NodeTSCAndHotReload.addDebugger = HotReloader.addDebugger.bind(HotReloader);
CS.NodeTSCAndHotReload.removeDebugger = HotReloader.removeDebugger.bind(HotReloader);


teaCup.watch(__dirname + '/../tsconfig.json', (fileName, fileContent) => {
    // 确定最终输出目录
    const targetPath = __dirname + '/../../Assets/Samples/TSBehaviour/Resources/' + path.basename(fileName).replace('.js', '.mjs');
    // 写文件
    fs.writeFileSync(
        targetPath,
        fileContent,
        'utf-8'
    );
    // 通知hot-reload程序，文件有所变更
    HotReloader.emitFileChanged(targetPath);
});