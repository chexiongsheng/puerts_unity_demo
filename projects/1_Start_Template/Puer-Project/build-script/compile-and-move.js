const path = require('path');
const glob = require('glob');
const { mv, rm } = require('@puerts/shell-util')

// 执行tsconfig所定义的tsc行为
require('./@puer/tsc').compileTS(__dirname + '/../tsconfig.json')

// 将输出的js移动到对应目录
glob.sync(__dirname + '/../output/*.js').forEach(filename=> {
    mv(
        filename, 
        __dirname + '/../../Assets/Samples/TSBehaviour/Resources/' + path.basename(filename).replace('.js', '.mjs')
    )
})
rm('-rf', __dirname + "/../output");