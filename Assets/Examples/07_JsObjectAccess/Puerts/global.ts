import * as CS from 'csharp';
import { $typeof } from 'puerts';

function isCSharp(value: any) {
    return typeof (value['GetType']) !== 'undefined'
        && typeof (value['GetHashCode']) !== 'undefined';
}
function isValue(value: any) {
    var type = typeof (value);
    return type === 'string' || type === 'number' || type === 'boolean';
}
const numbers = [
    $typeof(CS.System.Int16), $typeof(CS.System.Int32), $typeof(CS.System.Int64),
    $typeof(CS.System.Single), $typeof(CS.System.Double), $typeof(CS.System.UInt16),
    $typeof(CS.System.UInt32), $typeof(CS.System.UInt64)];
function isNumber(type: CS.System.Type) {
    for (let i = 0; i < numbers.length; i++)
        if (numbers[i].Equals(type)) return true;
    return false;
}
function isCast(value: any, type: CS.System.Type) {
    if (isCSharp(value)) {
        return $typeof(CS.System.Object).Equals(type)
            || type.IsAssignableFrom((value as CS.System.Object).GetType());
    }
    if (isValue(value)) {
        var v_type = typeof (value);
        return $typeof(CS.System.Object).Equals(type)
            || v_type === 'number' && isNumber(type)
            || v_type === 'string' && $typeof(CS.System.String).Equals(type)
            || v_type === 'boolean' && $typeof(CS.System.Boolean).Equals(type);
    }
    if (typeof (value) == 'function') {
        return $typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
    }
    var j_type = $typeof(CS.Puerts.JsBase);
    while (type != null) {
        if (j_type.Equals(type))
            return true;
        type = type.BaseType;
    }
    return $typeof(CS.System.Object).Equals(type);
}

//funtion声明方法将在调用时绑定this(上下文), 使用()=>在声明时即绑定this
class GlobalRef {
    //Id索引
    private globalIdx = 0;
    //Env对象
    private globalEnv: CS.Puerts.JsEnv = null;
    //对象存储池
    private poolIds!: Map<string, { value: any, count: number }>;
    private poolObjs!: WeakMap<object, string>;

    init = (env: CS.Puerts.JsEnv) => {
        this.globalIdx = 0;
        this.globalEnv = env;
        this.poolIds = new Map();
        this.poolObjs = new WeakMap();
    }
    //create ref, return refid
    create = (obj: object | any[]) => {
        let id = this.poolObjs.get(obj);
        if (id === undefined || id === null || id === void 0) {
            id = this.globalEnv.Index + '_' + (++this.globalIdx).toString();
            this.poolIds.set(id, { value: obj, count: 0 });
            this.poolObjs.set(obj, id);
        }
        return id;
    }
    //add ref count
    ref = (id: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            ref.count++;
            return true;
        }
        return false;
    }
    //release ref count
    release = (id: string, count?: number) => {
        count = count ?? 1;
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            ref.count -= count;
            if (ref.count <= 0) {
                this.poolIds.delete(id);
                this.poolObjs.delete(ref.value);
            }
            return true;
        }
        return false;
    }
    //delete ref
    delete = (obj: object | string) => {
        if (obj === undefined || obj === null || obj === void 0)
            throw new Error('invalid parameters obj');

        let id = typeof (obj) === 'string' ? obj as string : this.poolObjs.get(obj as object);
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            this.poolIds.delete(id);
            this.poolObjs.delete(ref.value);
            return true;
        }
        return false;
    }
    toCSharp = (obj: object | any[]) => {
        if (obj === undefined || obj === null || obj === void 0)
            return null;
        if (typeof (obj) === 'function'
            || isValue(obj) || isCSharp(obj))
            return obj;
        //创建Id映射对象
        return new CS.Puerts.JsObject(this.create(obj), this.globalEnv);
    }
    //Get value by refId
    value = (id: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            return ref.value;
        }
        return undefined;
    }
    //Read and write
    get = (id: string, key: string | number) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            return this.toCSharp(ref.value[key]);
        }
        return null;
    }
    set = (id: string, key: string | number, value: any) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            ref.value[key] = value;
            return true;
        }
        return false;
    }
    getInPath = (id: string, path: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let tmp = ref.value;
            let nodes = path.split('.');
            let last = nodes.pop();
            nodes.forEach(n => {
                tmp = tmp[n];
                if (typeof (tmp) === 'undefined')
                    throw new Error('invalid path: ' + path);
            });
            return this.toCSharp(tmp[last]);
        }
        return null;
    }
    setInPath = (id: string, path: string, value: any) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let tmp = ref.value;
            let nodes = path.split('.');
            let last = nodes.pop();
            nodes.forEach(n => {
                tmp = tmp[n];
                if (typeof (tmp) === 'undefined')
                    throw new Error('invalid path: ' + path);
            });
            tmp[last] = value;
            return true;
        }
        return false;
    }
    //Cast to target type
    isCast = (id: string, key: string | number, type: CS.System.Type) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            var v = ref.value[key];
            if (v === undefined || v === null || v === void 0)
                return type.IsClass;

            return isCast(v, type);
        }
        return false;
    }
    isCastInPath = (id: string, path: string, type: CS.System.Type) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let tmp = ref.value;
            let nodes = path.split('.');
            let last = nodes.pop();
            for (let i = 0; i < nodes.length; i++) {
                tmp = tmp[nodes[i]];
                if (typeof (tmp) === 'undefined')
                    return false;
            }
            var v = tmp[last];
            if (v === undefined || v === null || v === void 0)
                return type.IsClass;

            return isCast(v, type);
        }
        return false;
    }
    contains = (id: string, path: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let tmp = ref.value;
            let nodes = path.split('.');
            let last = nodes.pop();
            for (let i = 0; i < nodes.length; i++) {
                tmp = tmp[nodes[i]];
                if (typeof (tmp) === 'undefined')
                    return false;
            }
            return typeof (tmp[last]) !== 'undefined';
        }
        return false;
    }
    //GetKeys
    keys = (id: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let keys = Object.keys(ref.value);
            let cs_keys = CS.System.Array.CreateInstance($typeof(CS.System.String), keys.length);
            for (let i = 0; i < keys.length; i++) {
                cs_keys.SetValue(keys[i], i);
            }
            return cs_keys;
        }
        return null;
    }
    //Length
    length = (id: string) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            return Object.keys(ref.value).length;
        }
        return -1;
    }
    //foreach
    foreach = (id: string, action: any, valueType?: CS.System.Type) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let value = ref.value;
            if (isCSharp(action)
                && $typeof(CS.System.MulticastDelegate).IsAssignableFrom(action.GetType())) {
                const func = action;
                action = (...args: any) => func.Invoke(...args);
            }
            Object.keys(value).forEach(key => { 
                let v = value[key];
                if (valueType === undefined || isCast(v, valueType)) {
                    action(key, this.toCSharp(v));
                }
            });
            return true;
        }
        return false;
    }
    //Call Function
    call = (id: string, name: string, args: any) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            let func = ref.value[name];
            if (typeof (func) !== 'function')
                throw new Error(name + ' is not funtion');
            //convert args
            let _args = [];
            for (let i = 0; i < args.length; i++)
                _args.push(args[i]);

            var result = (func as Function).call(ref.value, ..._args);
            return this.toCSharp(result);
        }
        return null;
    }
}

globalThis.$puertsRef = new GlobalRef();