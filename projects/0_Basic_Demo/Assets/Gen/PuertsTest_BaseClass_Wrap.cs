
using PuertsTest;

using System;
using Puerts;

namespace PuertsStaticWrap
{
    public static class PuertsTest_BaseClass_Wrap
    {

        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8ConstructorCallback))]
        private static IntPtr Constructor(IntPtr isolate, IntPtr info, int paramLen, long data)
        {
            try
            {

                {
            
                    {
                
                        var result = new PuertsTest.BaseClass();
                
                        return Puerts.Utils.GetObjectPtr((int)data, typeof(PuertsTest.BaseClass), result);
                    
                    }
                
                }
            
    
            } catch (Exception e) {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
            return IntPtr.Zero;
        }
    
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void F_BSFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
        
                {
            
                    {
                
                        PuertsTest.BaseClass.BSFunc();
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_BMFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
        
                {
            
                    {
                
                        obj.BMFunc();
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_PlainExtension(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
        
                {
            
                    {
                
                        obj.PlainExtension();
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_Extension1(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
        
                {
            
                    {
                
                        var result = obj.Extension1();
                
                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_Extension2(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
        
                if (paramLen == 1)
            
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    if (argHelper0.IsMatch(Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(UnityEngine.GameObject), false, false))
                
                    {
                
                        var Arg0 = argHelper0.Get<UnityEngine.GameObject>(false);
                    
                        var result = obj.Extension2(Arg0);
                
                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        
                        return;
                    }
                
                    if (argHelper0.IsMatch(Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(PuertsTest.BaseClass1), false, false))
                
                    {
                
                        var Arg0 = argHelper0.Get<PuertsTest.BaseClass1>(false);
                    
                        obj.Extension2(Arg0);
                
                        
                        
                        return;
                    }
                
                }
            
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to Extension2");
        
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void G_BMF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
                var result = obj.BMF;
                Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void S_BMF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.BaseClass;
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                obj.BMF = argHelper.GetInt32(false);
                
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void G_BSF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var result = PuertsTest.BaseClass.BSF;
                Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void S_BSF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                PuertsTest.BaseClass.BSF = argHelper.GetInt32(false);
                
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
                
        public static Puerts.TypeRegisterInfo GetRegisterInfo()
        {
            return new Puerts.TypeRegisterInfo()
            {
                BlittableCopy = false,
                Constructor = Constructor,
                Methods = new System.Collections.Generic.Dictionary<Puerts.MethodKey, Puerts.V8FunctionCallback>()
                {   
                    { new Puerts.MethodKey { Name = "BSFunc", IsStatic = true}, F_BSFunc },
                    { new Puerts.MethodKey { Name = "BMFunc", IsStatic = false}, M_BMFunc },
                    { new Puerts.MethodKey { Name = "PlainExtension", IsStatic = false}, M_PlainExtension },
                    { new Puerts.MethodKey { Name = "Extension1", IsStatic = false}, M_Extension1 },
                    { new Puerts.MethodKey { Name = "Extension2", IsStatic = false}, M_Extension2 }
                },
                Properties = new System.Collections.Generic.Dictionary<string, Puerts.PropertyRegisterInfo>()
                {
                    
                    {"BMF", new Puerts.PropertyRegisterInfo(){ IsStatic = false, Getter = G_BMF, Setter = S_BMF} },

                    {"BSF", new Puerts.PropertyRegisterInfo(){ IsStatic = true, Getter = G_BSF, Setter = S_BSF} }
                },
                LazyMembers = new System.Collections.Generic.List<Puerts.LazyMemberRegisterInfo>()
                {   
                }
            };
        }
    
    }
}
