//@ts-ignore
import * as fs from "fs";
import * as path from "path";
import { EventEmitter } from "events";
import * as CDP from "chrome-remote-interface";
import * as os from "os";
const CDPFunc = require("chrome-remote-interface");

const MAX_SCRIPTS_CACHE_SIZE = 10000000;

type LockHandler = { release: () => void };
type ScriptParsedEvent = { scriptId: string, url: string };
type ScriptFailedToParseEvent = { scriptId: string, url: string };

let example: CDP.Client;
export class Debugger extends EventEmitter {
    public get state() { return this._state; }
    public get isOpend() { return this._state === State.Open; }

    private _trace: boolean;
    private _ignorePathCase: boolean;
    private _checkOnStartup: boolean;

    private _state: State = State.None;
    private _client: CDP.Client;
    private _debugger: typeof example.Debugger;
    //cache script infos
    private _scriptParsed: { [key: string]: string };
    private _scriptFailedToParse: { [key: string]: string };
    private _locks: Map<string, Lock>;

    constructor(params?: Partial<{ trace: boolean, ignorePathCase: boolean, checkOnStartup: boolean }>) {
        super();
        let { trace, ignorePathCase, checkOnStartup } = params ?? {};
        this._trace = trace ?? true;
        this._ignorePathCase = ignorePathCase ?? process.platform === "win32";
        this._checkOnStartup = checkOnStartup ?? true;
    }

    public async open(host: string, port?: number, local?: boolean) {
        if (this._trace) {
            console.log('open1');
        }
        if (this._state === State.Connecting || this._state === State.Open)
            throw new Error("socket is opening");

        this.close();
        if (typeof local === "undefined")
            local = true;
        if (this._trace) {
            console.log('open2');
        }
        try {
            this._state = State.Connecting;
            this._client = await (CDPFunc as any)({ host, port, local });
        if (this._trace) {
            console.log('open3');
        }
        const { Runtime, Debugger } = this._client;
            this._debugger = Debugger;

            Debugger.on("scriptParsed", this._scriptParsedHanlder);
            Debugger.on("scriptFailedToParse", this._scriptFailedToParseHandler);

            await Runtime.enable();
        if (this._trace) {
            console.log('open4');
        }
        await Debugger.enable({ "maxScriptsCacheSize": MAX_SCRIPTS_CACHE_SIZE });
        if (this._trace) {
            console.log('open5');
        }

            this._client.on("disconnect", this._disconnectHandler);

            this._state = State.Open;
        } catch (err) {
            console.error(err);
            this.close();
            return "CONNECT_FAIL: " + err;
        }
        return undefined;
    }

    public async close() {
        this._state = State.Close;
        this._scriptParsed = undefined;
        this._scriptFailedToParse = undefined;
        this._debugger = undefined;
        this._locks = undefined;

        if (this._client) {
            let client = this._client;
            this._client = undefined;
            await client.close();
        }
    } 

    public update(filepath: string) {
        if (this._state !== State.Open || !this._debugger)
            throw new Error("socket is close");

        if (this._trace) {
            console.log(`change: ${filepath}`);
        }
        if (!this._scriptParsed)
            return;

        filepath = path.normalize(filepath).replace(/\\/g, "/");;
        if (this._ignorePathCase) filepath = filepath.toLowerCase();
        this._pushUpdate(filepath);
    }

    private async _pushUpdate(filepath: string) {
        if (this._trace) {
            console.log('update debugger filePath:' + filepath);
        }
        let scriptId = this._scriptParsed[filepath] || this._scriptParsed[(os.platform() == 'win32' ? "file:/" : "file:") + filepath];
        if (this._trace) {
            console.log('ScriptId:' + scriptId);
            console.log(JSON.stringify(this._scriptParsed));
        }
        if (scriptId && fs.existsSync(filepath) && fs.lstatSync(filepath).isFile()) {
            if (this._trace) {
                console.log("===================Do Update Script!!!");
            }
            let scriptSource = fs.readFileSync(filepath).toString("utf-8");
            scriptSource = ("(function (exports, require, module, __filename, __dirname) { " + scriptSource + "\n});");

            let lock = await this._lock(scriptId);
            try {
                await this.sendUpdateMessage(scriptId, filepath, scriptSource);
            } catch (error) {
                console.error(error);
            }
            lock.release();
        }
    }

    private async sendUpdateMessage(scriptId: any, filepath: string, scriptSource: string)
    {
        if (!this._debugger)
            return;

        if (this._scriptParsed[(os.platform() == 'win32' ? "file:/" : "file:") + filepath])
        {
            filepath = (os.platform() == 'win32' ? "file:/" : "file:") + filepath;
        }
        if (this._trace) {
            console.log("===================Has The Lock!!!");
        }
        if (this._trace) console.log(`check: \t${scriptId}:${filepath}`);
        let exist = await this._debugger.getScriptSource({ scriptId });
        if (!exist || exist.scriptSource === scriptSource || !this._debugger)
            return;

        if (this._trace) console.log(`send: \t${scriptId}:${filepath}:${scriptSource}`);
        let response = await this._debugger.setScriptSource({ scriptId, scriptSource });
        if (this._trace) {
            console.log(`completed: \t${scriptId}:${filepath}` + `| \t${JSON.stringify(response)}`);
        } 
    }

    private _scriptParsedHanlder = (params: ScriptParsedEvent) => {
        if (this._trace) {
            console.log("============ScriptParsed!!!");
        }
        if (!params || !params.url || !params.scriptId)
            return;

        let scriptId = params.scriptId;
        if (this._trace) {
            console.log("Parsed ScriptId: " + scriptId);
        }
        let filepath = path.normalize(params.url).replace(/\\/g, "/");;
        if (this._ignorePathCase) filepath = filepath.toLowerCase();
        if (this._trace) {
            console.log(`scriptParsed: ${scriptId}:${filepath}`);
        }

        if (!this._scriptParsed)
            this._scriptParsed = {};
        this._scriptParsed[filepath] = scriptId;
        this._scriptParsed[scriptId] = filepath;

        if (this._checkOnStartup)
            this._pushUpdate(filepath);
    };
    private _scriptFailedToParseHandler = (params: ScriptFailedToParseEvent) => {
        if (!params || !params.url || !params.scriptId)
            return;

        let scriptId = params.scriptId;
        let url = path.normalize(params.url).replace(/\\/g, "/");;
        if (this._ignorePathCase) url = url.toLowerCase();
        if (this._trace) {
            console.log(`scriptFailedToParse: ${scriptId}:${url}`);
        }

        if (!this._scriptFailedToParse)
            this._scriptFailedToParse = {};
        this._scriptFailedToParse[url] = scriptId;
        this._scriptFailedToParse[scriptId] = url;
    };
    private _disconnectHandler = () => {
        this.close();
        this.emit("disconnect");
    };

    private async _lock(key: string) {
        if (!this._locks)
            this._locks = new Map();
        let lock = this._locks.get(key);
        if (!lock) {
            lock = new Lock();
            this._locks.set(key, lock);
        }
        return lock.acquire(0);
    }
}

export enum State {
    None,
    Connecting,
    Open,
    Close,
}

class Lock {
    public get isLocked() { return this._isLocked; }
    private _isLocked: boolean;                 //是否正在锁定中

    private _handlers: Array<{ priority: number, resolve: () => void }>;   //异步等待回调 
    private _handler: LockHandler;

    public async acquire(priority: number = 0): Promise<LockHandler> {
        if (priority && priority > 0) {
            await new Promise<void>(function (resolve) {
                setTimeout(resolve, priority);
            })
        }

        await this.acquireLock(priority ?? 0);

        this._handler = {
            release: () => {
                this._isLocked = false;
                this._handler = undefined;
                this.moveNext();
            }
        };
        return this._handler;
    }
    public reset() {
        this._isLocked = false;
        this._handlers = undefined;
        if (this._handler) {
            this._handler.release = function () { };
            this._handler = undefined;
        }
    }

    private async acquireLock(priority: number) {
        while (this._isLocked) {
            await new Promise<void>((resolve) => {
                if (!this._handlers)
                    this._handlers = [];
                this._handlers.push({ priority, resolve });
            });
        }
        this._isLocked = true;
    }
    private moveNext() {
        if (!this._handlers || this._handlers.length === 0)
            return;

        this._handlers.sort((o1, o2) => o1.priority < o2.priority ? -1 : o1.priority > o2.priority ? 1 : 0);
        this._handlers.shift().resolve();
    }
}
