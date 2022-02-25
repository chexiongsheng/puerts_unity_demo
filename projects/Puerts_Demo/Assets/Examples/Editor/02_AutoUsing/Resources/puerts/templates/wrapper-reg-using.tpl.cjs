/**
 * this template file is write for generating the wrapper register code
 * 
 * @param {GenInfo[]} infos
 * @returns 
 */
module.exports = function AutoRegTemplate(infos) {
    infos = toJsArray(infos);
    return `
namespace PuertsStaticWrap
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using JsEnv = Puerts.JsEnv;
    using BindingFlags = System.Reflection.BindingFlags;

    public static class AutoStaticCodeUsing
    {
        public static void AutoUsing(this JsEnv jsEnv)
        {
${
    infos.map(info => {
        return '            ' +
            `jsEnv.${info.Name}<${toJsArray(info.Parameters).join(', ')}>();`;
    }).join("\n")
}
        }

        public static void UsingAction(this JsEnv jsEnv, params string[] args)
        {
            jsEnv.UsingGeneric(true, FindTypes(args));
        }
        public static void UsingFunc(this JsEnv jsEnv, params string[] args)
        {
            jsEnv.UsingGeneric(false, FindTypes(args));
        }
        public static void UsingGeneric(this JsEnv jsEnv, bool usingAction, params Type[] types)
        {
            var name = usingAction ? "UsingAction" : "UsingFunc";
            var count = types.Length;
            var method = (from m in typeof(JsEnv).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          where m.Name.Equals(name)
                             && m.IsGenericMethod
                             && m.GetGenericArguments().Length == count
                          select m).FirstOrDefault();
            if (method == null)
                throw new Exception("Not found method: '" + name + "', ArgsLength=" + count);
            method.MakeGenericMethod(types).Invoke(jsEnv, null);
        }
        static Type[] FindTypes(string[] args)
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            var types = new List<Type>();
            foreach (var arg in args)
            {
                Type type = null;
                for (var i = 0; i < assemblys.Length && type == null; i++)
                    type = assemblys[i].GetType(arg, false);
                if (type == null)
                    throw new Exception("Not found type: '" + arg + "'");
                types.Add(type);
            }
            return types.ToArray();
        }
    }
}

    `.trim();
};

function toJsArray(csArr) {
    let arr = [];
    for (var i = 0; i < csArr.Length; i++) {
        arr.push(csArr.get_Item(i));
    }
    return arr;
}
