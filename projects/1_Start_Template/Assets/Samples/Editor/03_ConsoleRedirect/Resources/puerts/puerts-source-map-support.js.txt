const csharp = (function () {
    let csharp = (this ?? globalThis)["CS"];
    if (!csharp) csharp = require("csharp");
    return csharp;
})();
const puerts = (function () {
    let puerts = (this ?? globalThis)["puerts"];
    if (!puerts) puerts = require("puerts");
    return puerts;
})();

function hasRequireInterfaces() {
    try {
        let modules = [
            ["path", "dirname", "resolve"],
            ["fs", "existsSync", "readFileSync"]
        ];
        for (let [moduleName, ...methodNames] of modules) {
            let module = require(moduleName);
            if (!module)
                return false;
            if (methodNames.find(m => typeof (module[m]) !== "function"))
                return false;
        }
    } catch (e) {
        return false;
    }
    return true;
}

/**模拟source-map-support依赖的nodejs io模块(部分)接口 */
function registerSimulationInterfaces() {
    //过滤路径中的非法字符: 适用于webpack等打包工具
    const illegalCharacters = (function () {
        //characters not supported on microsoft windows platform
        if (csharp.UnityEngine.Application.platform === csharp.UnityEngine.RuntimePlatform.WindowsEditor ||
            csharp.UnityEngine.Application.platform === csharp.UnityEngine.RuntimePlatform.WindowsPlayer) {
            return ['"'];
        }
        return ['*', '?', '"', '<', '>', '|'];
    })();

    puerts.registerBuildinModule("path", {
        /**Return the directory name of a path. Similar to the Unix dirname command.
         * @param path {string}
         * @returns 
         */
        dirname(path) {
            if (illegalCharacters.find(keyword => path.includes(keyword))) {
                return '';
            }
            return csharp.System.IO.Path.GetDirectoryName(path);
        },
        /**
         * The right-most parameter is considered {to}. Other parameters are considered an array of {from}.
         * Starting from leftmost {from} parameter, resolves {to} to an absolute path.
         *
         * @param paths {string[]} A sequence of paths or path segments.
         * @returns 
         */
        resolve(...paths) {
            if (illegalCharacters.find(keyword => paths.find(path => path.includes(keyword)))) {
                return '';
            }
            return csharp.System.IO.Path.GetFullPath(csharp.System.IO.Path.Combine(...paths));
        },
    });
    puerts.registerBuildinModule("fs", {
        /**Returns `true` if the path exists, `false` otherwise.
         * @param path {string}
         * @returns 
         */
        existsSync(path) {
            return csharp.System.IO.File.Exists(path);
        },
        /**Returns the contents of the `path`.
         * @param path {string}
         * @returns 
         */
        readFileSync(path) {
            return csharp.System.IO.File.ReadAllText(path);
        },
    });
}

try {
    if (!hasRequireInterfaces()) {
        registerSimulationInterfaces();
        csharp.UnityEngine.Debug.Log("simulation interfaces: <color=green>registered</color>");
    }

    (function () {
        let global = this ?? globalThis;
        //global["Buffer"] = global["Buffer"] ?? {};
        //inlineSourceMap需要额外安装buffer库
        global["Buffer"] = global["Buffer"] ?? require("buffer").Buffer;
    })();
    require("source-map-support").install();

    csharp.UnityEngine.Debug.Log("source-map-support: <color=green>enable</color>");
} catch (e) {
    csharp.UnityEngine.Debug.LogError("source-map-support module exception:" + e.message + "\n" + e.stack);
}