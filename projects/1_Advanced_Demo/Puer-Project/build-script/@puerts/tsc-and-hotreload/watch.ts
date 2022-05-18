import * as fs from "fs";
import * as ts from "typescript";
import * as path from "path";
import * as mkdirp from "mkdirp";
import * as glob from "glob";
import { Debugger } from './lib/debugger';

export default class Watcher {
  constructor(tsConfigPath: string) {

    const cl: ts.ParsedCommandLine | undefined = ts.getParsedCommandLineOfConfigFile(
      tsConfigPath,
      {},
      Object.assign({ onUnRecoverableConfigFileDiagnostic: (d: any) => d }, ts.sys)
    );
    if (!cl) {
      throw new Error('parse tsconfig failed: ' + tsConfigPath);
    }


    const files: string[] = cl.fileNames.reduce((prev: string[], cur: string) => {
      return prev.concat(glob.sync(cur))
    }, []);

    watch(files, cl.options, this);
    console.log('watch fileCount:' + files.length);
    console.log('watch files:' + JSON.stringify(files));
  }

  private debuggers: { [key: number]: Debugger } = {};

  addDebugger(debuggerPort: number) {
    this.debuggers[debuggerPort] = new Debugger({
      checkOnStartup: true,
      trace: true
    });
    this.debuggers[debuggerPort].open("127.0.0.1", debuggerPort);
  }

  removeDebugger(debuggerPort: number) {
    if (this.debuggers[debuggerPort]) {
      this.debuggers[debuggerPort].close();
      delete this.debuggers[debuggerPort];
    }
  }

  emitFileChanged(filepath: string) {
    Object.keys(this.debuggers).forEach((key: string) => {

      try {
        this.debuggers[+key].update(filepath);
      } catch (e: any) {
        console.error((e as Error).stack)
      }
    })
  }
}

function watch(rootFileNames: string[], options: ts.CompilerOptions, watcher: Watcher) {
  const files: ts.MapLike<{ version: number }> = {};

  // initialize the list of files
  rootFileNames.forEach(fileName => {
    files[fileName] = { version: 0 };
  });

  // Create the language service host to allow the LS to communicate with the host
  const servicesHost: ts.LanguageServiceHost = {
    getScriptFileNames: () => rootFileNames,
    getScriptVersion: fileName =>
      files[fileName] && files[fileName].version.toString(),
    getScriptSnapshot: fileName => {
      if (!fs.existsSync(fileName)) {
        return undefined;
      }

      return ts.ScriptSnapshot.fromString(fs.readFileSync(fileName).toString());
    },
    getCurrentDirectory: () => process.cwd(),
    getCompilationSettings: () => options,
    getDefaultLibFileName: options => ts.getDefaultLibFilePath(options),
    fileExists: ts.sys.fileExists,
    readFile: ts.sys.readFile,
    readDirectory: ts.sys.readDirectory,
    directoryExists: ts.sys.directoryExists,
    getDirectories: ts.sys.getDirectories,
  };

  // Create the language service files
  const services = ts.createLanguageService(servicesHost, ts.createDocumentRegistry());

  // Now let's watch the files
  rootFileNames.forEach(fileName => {
    // First time around, emit all files
    emitFile(fileName);

    // Add a watch on the file to handle next change
    fs.watchFile(fileName, { persistent: true, interval: 250 }, (curr, prev) => {
      // Check timestamp
      if (+curr.mtime <= +prev.mtime) {
        return;
      }

      // Update the version to signal a change in the file
      files[fileName].version++;

      // write the changes to disk
      emitFile(fileName);
    });
  });

  function emitFile(fileName: string) {
    let output = services.getEmitOutput(fileName);

    if (!output.emitSkipped) {
      console.log('compiled ts:', fileName);
    } else {
      logErrors(fileName);
    }

    output.outputFiles.forEach(o => {
      mkdirp.sync(path.dirname(o.name));
      fs.writeFileSync(o.name, o.text, "utf8");
      watcher.emitFileChanged(o.name);
    });
  }

  function logErrors(fileName: string) {
    let allDiagnostics = services
      .getCompilerOptionsDiagnostics()
      .concat(services.getSyntacticDiagnostics(fileName))
      .concat(services.getSemanticDiagnostics(fileName));

    allDiagnostics.forEach(diagnostic => {
      let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n");
      if (diagnostic.file) {
        let { line, character } = diagnostic.file.getLineAndCharacterOfPosition(
          diagnostic.start!
        );
        console.log(`  Error ${diagnostic.file.fileName} (${line + 1},${character + 1}): ${message}`);
      } else {
        console.log(`  Error: ${message}`);
      }
    });
  }
}
