const sx = require('shelljs');
const iconv = require('iconv-lite')
const { program } = require('commander');
const path = require('path')
const fs = require('fs');
const glob = require('glob');
const download = require('download')
const sxExecAsync = async function (command) {
    return new Promise(resolve => {
        options.async = true;
        let child = sx.exec(command, {
            async: true,
            silent: true,
            encoding: 'binary'
        }, resolve);
        child.stdout.on('data', function (data) {
            console.log(iconv.decode(data, process.platform == 'win32' ? "gb2312" : 'utf-8'));
        })
        child.stderr.on('data', function (data) {
            console.error(iconv.decode(data, process.platform == 'win32' ? "gb2312" : 'utf-8'));
        })
    })
}
program.requiredOption("--version <version>", "the JS backend will be used");
program.parse(process.argv);
const options = program.opts();

const TEMP_PATH = `${__dirname}/../.temp`

;(async function() {
    console.log('[Puer] cleaning');
    sx.rm("-rf", TEMP_PATH);
    
    console.log('[Puer] downloading');
    const downNodeJS = download(`https://github.com/Tencent/puerts/releases/download/Unity_v${options.version}/PuerTS_Nodejs_${options.version}.tgz`, path.join(TEMP_PATH, 'nodejs'), { extract: true });
    // await Promise.all([downV8, downNodeJS]);
    await downNodeJS;

    console.log('[Puer] merging');
    sx.mkdir("-p", path.join(TEMP_PATH, 'package'));
    sx.cp("-r", path.join(TEMP_PATH, 'nodejs/Puerts/*'), path.join(TEMP_PATH, 'package'));
    
    glob.sync(path.join(__dirname, '/../package/*').replace(/\\/g, '/')).forEach(filepath=> {
        // files like changelog/LICENSE
        sx.cp(filepath, path.join(TEMP_PATH, 'package', path.basename(filepath)))
    })
    glob.sync(path.join(__dirname, '/../package/**/*.meta').replace(/\\/g, '/')).forEach(filepath=> {
        const relative = path.relative(path.join(__dirname, '/../package/'), filepath);
        if (fs.existsSync(path.dirname(path.join(TEMP_PATH, 'package', relative)))) {
            sx.cp(filepath, path.dirname(path.join(TEMP_PATH, 'package', relative)))
        }
    });

    console.log('[Puer] update package.json');
    const packageJSON = JSON.parse(fs.readFileSync(path.join(TEMP_PATH, 'package', 'package.json')));
    packageJSON.version = options.version;
    fs.writeFileSync(
        path.join(TEMP_PATH, 'package', 'package.json'), 
        JSON.stringify(packageJSON),
        'utf-8'
    );

    console.log('[Puer] copying');
    try {
        sx.rm('-rf', path.join(__dirname, '/../package'));
    } catch(e) {}
    sx.rm('-rf', path.join(__dirname, '/../package'));
    sx.cp('-r', path.join(TEMP_PATH, 'package'), path.join(__dirname, "/../"))
    
    console.log('[Puer] cleaning');
    sx.rm("-rf", TEMP_PATH);
})()
.catch(err=> console.error(err));