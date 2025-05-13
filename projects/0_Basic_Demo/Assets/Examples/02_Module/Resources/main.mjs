//import module1 from "module1.mjs";
//const module1 = CS.PuertsTest.Module.GetJsEnv().ExecuteModule("module1.mjs");
const module1 =  globalThis.__puertsExecuteModule("module1.mjs");
console.log(typeof module1);
console.log(module1.then);
module1.callMe('from john');
