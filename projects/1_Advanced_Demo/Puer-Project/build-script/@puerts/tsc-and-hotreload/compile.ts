import ts from 'typescript';

declare const puertsRequire: any;
const CS = puertsRequire('csharp')

export default function(tsConfigPath: string) {
    CS.UnityEditor.EditorUtility.DisplayProgressBar("complile ts", "compiling typescript", 0.5);
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