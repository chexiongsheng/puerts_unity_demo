import { program } from 'commander';
import path from 'path';
import fs from 'fs';
import glob from 'glob';
import download from 'download'
import { mkdir, rm, mv, cp } from "@puerts/shell-util"
const pwd = process.cwd();

if (!fs.existsSync(`${pwd}/package.json`)) {
    console.error("[Puer] invalid directory, please run in the root of PerformanceTesting");
    process.exit(1);``
}
if (!fs.existsSync(`${pwd}/node_modules`)) {
    console.log("[Puer] installing node_modules");
    require('child_process').execSync('npm i')
}

program.requiredOption("--version <version>", "the JS backend will be used");
program.option("--org <org>", "where to download the puerts artifact", "Tencent");
program.option("--path <path>", "put the puerts package to", "package")

program.parse(process.argv);
const options = program.opts();

const TEMP_PATH = `${process.cwd()}/.temp`.replace(/\\/g, '/')
const TARGET_PATH = `${process.cwd()}/${options.path}`.replace(/\\/g, '/')

async function run() {
    console.log('[Puer] cleaning');
    rm("-rf", TEMP_PATH);

    console.log('[Puer] downloading');
    const downNodeJS = download(`https://github.com/${options.org}/puerts/releases/download/Unity_v${options.version}/PuerTS_NodeJS_${options.version}.tgz`, path.join(TEMP_PATH, 'remote'), { extract: true });
    // await Promise.all([downV8, downNodeJS]);
    await downNodeJS;

    console.log('[Puer] merging');
    mkdir("-p", path.join(TEMP_PATH, 'local'));
    cp("-r", path.join(TEMP_PATH, 'remote/upm/*'), path.join(TEMP_PATH, 'local'));

    // inherit files like changelog/LICENSE
    glob.sync(path.join(TARGET_PATH, '/*').replace(/\\/g, '/')).forEach(filepath => {
        cp(filepath, path.join(TEMP_PATH, 'local', path.basename(filepath)))
    })
    glob.sync(path.join(TARGET_PATH, '/**/*.meta').replace(/\\/g, '/')).forEach(filepath => {
        const relative = path.relative(path.join(TARGET_PATH, '/'), filepath);
        if (fs.existsSync(path.dirname(path.join(TEMP_PATH, 'local', relative)))) {
            cp(filepath, path.dirname(path.join(TEMP_PATH, 'local', relative)))
        }
    });

    console.log('[Puer] update package.json');
    const packageJSON = fs.existsSync(path.join(TEMP_PATH, 'local', 'package.json')) ?
        JSON.parse(fs.readFileSync(path.join(TEMP_PATH, 'local', 'package.json'))) : defaultPuertsPackageJSON;
    packageJSON.version = options.version;
    fs.writeFileSync(
        path.join(TEMP_PATH, 'local', 'package.json'),
        JSON.stringify(packageJSON),
        'utf-8'
    );

    console.log('[Puer] moving');
    try {
        rm('-rf', TARGET_PATH);
    } catch (e) { }
    mv(path.join(TEMP_PATH, 'local'), TARGET_PATH)

    console.log('[Puer] cleaning');
    rm("-rf", TEMP_PATH);
}

const defaultPuertsPackageJSON = {
    "name": "com.tencent.puerts.core",
    "displayName": "Puerts",
    "description": "Write your game with TypeScript in UE4 or Unity. Puerts can be pronounced as pu-erh TS（普洱TS）",
    "keywords": [
        "Script"
    ],
    "author": {
        "name": "Tencent Puerts Group",
        "email": "zombieyang@tencent.com",
        "url": "https://github.com/puerts"
    },
    "category": "Libraries"
};

run().catch(err => console.error(err));