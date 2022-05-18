import ts from 'typescript';
import fs from 'fs';
import mkdirp from 'mkdirp';
import path from 'path';
import glob from 'glob';

declare const puertsRequire: any;
const CS = puertsRequire('csharp')

export default function (tsConfigPath: string) {
    CS.UnityEditor.EditorUtility.DisplayProgressBar("compile ts", "compiling typescript", 0.5);
    const cl: ts.ParsedCommandLine | undefined = ts.getParsedCommandLineOfConfigFile(
        tsConfigPath,
        {},
        Object.assign({ onUnRecoverableConfigFileDiagnostic: (d: any) => d }, ts.sys)
    );
    if (!cl) {
        throw new Error('parse tsconfig failed: ' + tsConfigPath);
    }

    const program = ts.createProgram(cl.fileNames, cl.options);
    let emitResult = program.emit();

    let allDiagnostics = ts
        .getPreEmitDiagnostics(program)
        .concat(emitResult.diagnostics);

    allDiagnostics.forEach(diagnostic => {
        if (diagnostic.file) {
            let { line, character } = ts.getLineAndCharacterOfPosition(diagnostic.file, diagnostic.start!);
            let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n");
            console.log(`${diagnostic.file.fileName} (${line + 1},${character + 1}): ${message}`);
        } else {
            console.log(ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n"));
        }
    });
    CS.UnityEditor.EditorUtility.ClearProgressBar();
}

export function watch(tsConfigPath, onFileChanged = (name: string) => { }) {
    const cl: ts.ParsedCommandLine | undefined = ts.getParsedCommandLineOfConfigFile(
        tsConfigPath,
        {},
        Object.assign({ onUnRecoverableConfigFileDiagnostic: (d: any) => d }, ts.sys)
    );
    if (!cl) {
        throw new Error('parse tsconfig failed: ' + tsConfigPath);
    }

    const files: ts.MapLike<{ version: number }> = {};
    const rootFileNames = cl.fileNames.reduce((prev: string[], cur: string) => {
        return prev.concat(glob.sync(cur))
      }, []);
    const options = cl.options;
    console.log('watch fileCount:' + files.length);
    console.log('watch files:' + JSON.stringify(files));

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
            onFileChanged(o.name);
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