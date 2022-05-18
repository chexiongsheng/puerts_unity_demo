import * as ts from "typescript";
import * as glob from "glob";
import { Debugger } from './lib/debugger';
import { watch } from "./run-ts";

export default class Watcher {
  constructor(tsConfigPath: string) {
    watch(tsConfigPath, this.emitFileChanged.bind(this))
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
      } catch (e) {
        console.error(e.stack)
      }
    })
  }
}