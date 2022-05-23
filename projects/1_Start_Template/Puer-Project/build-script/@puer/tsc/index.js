require('ts-node').register({ project: `${__dirname}/tsconfig.json` });

const tsEntry = require('./run-tsc.ts')

exports.compileTS = tsEntry.default
exports.watch = tsEntry.watch

