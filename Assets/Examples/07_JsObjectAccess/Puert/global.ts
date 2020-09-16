import * as CS from 'csharp';
import { $typeof } from 'puerts';

function isCSharpType(arg: any) {
    return typeof (arg['GetType']) !== 'undefined'
        && typeof (arg['GetHashCode']) !== 'undefined';
}
function isValueType(arg: any) {
    var type = typeof (arg);
    return type === 'string' || type === 'number' || type === 'boolean';
}
const types = [
    $typeof(CS.System.Int16), $typeof(CS.System.Int32), $typeof(CS.System.Int64),
    $typeof(CS.System.Single), $typeof(CS.System.Double), $typeof(CS.System.UInt16),
    $typeof(CS.System.UInt32), $typeof(CS.System.UInt64)];
function isNumber(type: CS.System.Type) {
    for (let i = 0; i < types.length; i++)
        if (types[i].Equals(type))
            return true;
    return false;
}

//funtion声明方法将在调用时绑定this(上下文), 使用()=>在声明时即绑定this
class GlobalRef {
    //Id索引
    private globalIdx = 0;
    //Env对象
    private globalEnv: CS.Puerts.JsEnv = null;
    //对象存储池
    private poolIds!: Map<number, { value: any, count: number }>;
    private poolObjs!: WeakMap<object, number>;

    init = (env: CS.Puerts.JsEnv) => {
        this.globalIdx = 0;
        this.globalEnv = env;
        this.poolIds = new Map();
        this.poolObjs = new WeakMap();
    }
    //为对象创建引用, 并返回引用Id
    create = (obj: object | any[]) => {
        let id = this.poolObjs.get(obj);
        if (id === undefined || id === null || id === void 0) {
            id = ++this.globalIdx;
            this.poolIds.set(id, { value: obj, count: 0 });
            this.poolObjs.set(obj, id);

            if (this.globalIdx >= Number.MAX_VALUE)
                this.globalIdx = Number.MIN_VALUE;
        }
        return { id: id, ref: this.poolIds.get(id) };
    }
    //引用对象 
    ref = (id: number) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            ref.count++;
            return true;
        }
        return false;
    }
    //回收对象
    release = (id: number, count?: number) => {
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
    //删除对象引用
    delete = (obj: object | number) => {
        if (obj === undefined || obj === null || obj === void 0)
            throw new Error('invalid parameters obj');

        let id = typeof (obj) === 'number' ? obj as number : this.poolObjs.get(obj as object);
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            this.poolIds.delete(id);
            this.poolObjs.delete(ref.value);
            return true;
        }
        return false;
    }
    //Js对象传递给C#
    toCSharp = (obj: object | any[]) => {
        if (obj === undefined || obj === null || obj === void 0)
            return null;

        if (isValueType(obj) || isCSharpType(obj))
            return obj;
        if (typeof (obj) === 'function')
            return obj;

        let { id, ref } = this.create(obj);
        return new CS.Puerts.JsObject(id, this.globalEnv);
    }
    //取出id对应的对象
    value = (id: number) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            return ref.value;
        }
        return undefined;
    }
    //读取键值
    get = (id: number, key: string | number) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            return this.toCSharp(ref.value[key]);
        }
        return null;
    }
    //写入键值
    set = (id: number, key: string | number, value: any) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            ref.value[key] = value;
            return true;
        }
        return false;
    }
    //能否转换为目标对象
    isCast = (id: number, key: string | number, type: CS.System.Type) => {
        let ref = this.poolIds.get(id);
        if (ref !== undefined && ref != null && ref != void 0) {
            var v = ref.value[key];
            if (v === undefined || v === null || v === void 0)
                return type.IsClass;
            if (isCSharpType(v))
                return type.IsAssignableFrom((v as CS.System.Object).GetType());
            if (isValueType(v)) {
                var t = typeof (v);
                return t === 'number' && isNumber(type)
                    || t === 'boolean' && type.Equals($typeof(CS.System.Boolean))
                    || t === 'string' && type.Equals($typeof(CS.System.String));
            }
            if (typeof (v) == 'function') {
                return $typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
            }
            var j_type = $typeof(CS.Puerts.JsBase);
            while (type != null) {
                if (type === j_type)
                    return true;
                type = type.BaseType;
            }
        }
        return false;
    }
    //路径设置 
    getInPath = (id: number, path: string) => {
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
    setInPath = (id: number, path: string, value: any) => {
        if (id === undefined || id === null || id === void 0)
            throw new Error('invalid parameters id');

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
    isCastInPath = (id: number, path: string, type: CS.System.Type) => {
        if (id === undefined || id === null || id === void 0)
            throw new Error('invalid parameters id');

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

            var v = tmp[last];
            if (v === undefined || v === null || v === void 0)
                return type.IsClass;
            if (isCSharpType(v))
                return type.IsAssignableFrom((v as CS.System.Object).GetType());
            if (isValueType(v)) {
                var t = typeof (v);
                return t === 'number' && isNumber(type)
                    || t === 'boolean' && type.Equals($typeof(CS.System.Boolean))
                    || t === 'string' && type.Equals($typeof(CS.System.String));
            }
            if (typeof (v) == 'function') {
                return $typeof(CS.System.MulticastDelegate).IsAssignableFrom(type);
            }
            var j_type = $typeof(CS.Puerts.JsBase);
            while (type != null) {
                if (type === j_type)
                    return true;
                type = type.BaseType;
            }
        }
        return false;
    }
    //对象中存在路径
    contains = (id: number, path: string) => {
        if (id === undefined || id === null || id === void 0)
            throw new Error('invalid parameters id');
        var t = typeof (path);
        if (t !== 'number' && t != 'string')
            throw new Error('invalid parameters key');

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
}

globalThis.$puertsRef = new GlobalRef();