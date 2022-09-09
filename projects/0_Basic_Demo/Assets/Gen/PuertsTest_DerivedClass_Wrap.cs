
using System;
using Puerts;

namespace PuertsStaticWrap
{
    public static class PuertsTest_DerivedClass_Wrap 
    {

        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8ConstructorCallback))]
        private static IntPtr Constructor(IntPtr isolate, IntPtr info, int paramLen, long data)
        {
            try
            {

                {
            
                    {
                
                        var result = new PuertsTest.DerivedClass();
                
                        return Puerts.Utils.GetObjectPtr((int)data, typeof(PuertsTest.DerivedClass), result);
                    
                    }
                
                }
            
    
            } catch (Exception e) {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
            return IntPtr.Zero;
        }
    
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void F_DSFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
        
                {
            
                    {
                
                        PuertsTest.DerivedClass.DSFunc();
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_DMFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                if (paramLen == 0)
            
                {
            
                    {
                
                        obj.DMFunc();
                
                        
                        
                        return;
                    }
                
                }
            
                if (paramLen == 1)
            
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    if (argHelper0.IsMatch(Puerts.JsValueType.Number, typeof(PuertsTest.MyEnum), false, false))
                
                    {
                
                        var Arg0 = (PuertsTest.MyEnum)argHelper0.GetInt32(false);
                    
                        var result = obj.DMFunc(Arg0);
                
                        Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, (int)result);
                        
                        return;
                    }
                
                }
            
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to DMFunc");
        
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_Trigger(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    {
                
                        obj.Trigger();
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_ParamsFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                if (paramLen >= 1)
            
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    var argHelper1 = new Puerts.ArgumentHelper((int)data, isolate, info, 1);
                
                    if (argHelper0.IsMatch(Puerts.JsValueType.Number, typeof(int), false, false) && argHelper1.IsMatchParams(Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.String, typeof(string), 1, paramLen))
                
                    {
                
                        var Arg0 = argHelper0.GetInt32(false);
                    
                        var Arg1 = argHelper1.GetParams<string>(info, 1, paramLen);
                    
                        var result = obj.ParamsFunc(Arg0, Arg1);
                
                        Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
                        
                        return;
                    }
                
                }
            
                if (paramLen == 1)
            
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    if (argHelper0.IsMatch(Puerts.JsValueType.Number, typeof(int), false, false))
                
                    {
                
                        var Arg0 = argHelper0.GetInt32(false);
                    
                        var result = obj.ParamsFunc(Arg0);
                
                        Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
                        
                        return;
                    }
                
                }
            
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to ParamsFunc");
        
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_InOutArgFunc(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    var argHelper1 = new Puerts.ArgumentHelper((int)data, isolate, info, 1);
                
                    var argHelper2 = new Puerts.ArgumentHelper((int)data, isolate, info, 2);
                
                    {
                
                        var Arg0 = argHelper0.GetInt32(false);
                    
                        var Arg1 = argHelper1.GetInt32(true);
                    
                        var Arg2 = argHelper2.GetInt32(true);
                    
                        var result = obj.InOutArgFunc(Arg0, out Arg1, ref Arg2);
                
                        argHelper1.SetByRefValue(Arg1);
                        
                        argHelper2.SetByRefValue(Arg2);
                        
                        Puerts.StaticTranslate<double>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_PrintList(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    {
                
                        var Arg0 = argHelper0.Get<System.Collections.Generic.List<int>>(false);
                    
                        obj.PrintList(Arg0);
                
                        
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_GetAb(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    {
                
                        var Arg0 = argHelper0.GetInt32(false);
                    
                        var result = obj.GetAb(Arg0);
                
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
        private static void M_SumOfAb(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    {
                
                        var Arg0 = argHelper0.Get<Puerts.ArrayBuffer>(false);
                    
                        var result = obj.SumOfAb(Arg0);
                
                        Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
                        
                        
                    }
                
                }
            
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void M_GetFileLength(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
        
                {
            
                    var argHelper0 = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                
                    {
                
                        var Arg0 = argHelper0.GetString(false);
                    
                        var result = obj.GetFileLength(Arg0);
                
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
        private static void G_DMF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var result = obj.DMF;
                Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void S_DMF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                obj.DMF = argHelper.GetInt32(false);
                
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void G_DSF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var result = PuertsTest.DerivedClass.DSF;
                Puerts.StaticTranslate<int>.Set((int)data, isolate, Puerts.NativeValueApi.SetValueToResult, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void S_DSF(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                PuertsTest.DerivedClass.DSF = argHelper.GetInt32(false);
                
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void G_MyCallback(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var result = obj.MyCallback;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void S_MyCallback(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                obj.MyCallback = argHelper.Get<PuertsTest.MyCallback>(false);
                
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void A_MyEvent(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                obj.MyEvent += argHelper.Get<PuertsTest.MyCallback>(false);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void R_MyEvent(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as PuertsTest.DerivedClass;
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                obj.MyEvent -= argHelper.Get<PuertsTest.MyCallback>(false);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void A_MyStaticEvent(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                PuertsTest.DerivedClass.MyStaticEvent += argHelper.Get<PuertsTest.MyCallback>(false);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
            
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        private static void R_MyStaticEvent(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                
                var argHelper = new Puerts.ArgumentHelper((int)data, isolate, info, 0);
                PuertsTest.DerivedClass.MyStaticEvent -= argHelper.Get<PuertsTest.MyCallback>(false);
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
                    { new Puerts.MethodKey { Name = "DSFunc", IsStatic = true}, F_DSFunc },
                    { new Puerts.MethodKey { Name = "DMFunc", IsStatic = false}, M_DMFunc },
                    { new Puerts.MethodKey { Name = "Trigger", IsStatic = false}, M_Trigger },
                    { new Puerts.MethodKey { Name = "ParamsFunc", IsStatic = false}, M_ParamsFunc },
                    { new Puerts.MethodKey { Name = "InOutArgFunc", IsStatic = false}, M_InOutArgFunc },
                    { new Puerts.MethodKey { Name = "PrintList", IsStatic = false}, M_PrintList },
                    { new Puerts.MethodKey { Name = "GetAb", IsStatic = false}, M_GetAb },
                    { new Puerts.MethodKey { Name = "SumOfAb", IsStatic = false}, M_SumOfAb },
                    { new Puerts.MethodKey { Name = "GetFileLength", IsStatic = false}, M_GetFileLength },

                    { new Puerts.MethodKey { Name = "add_MyEvent", IsStatic = false}, A_MyEvent },
                    { new Puerts.MethodKey { Name = "remove_MyEvent", IsStatic = false},  R_MyEvent },

                    { new Puerts.MethodKey { Name = "add_MyStaticEvent", IsStatic = true}, A_MyStaticEvent },
                    { new Puerts.MethodKey { Name = "remove_MyStaticEvent", IsStatic = true},  R_MyStaticEvent }
                },
                Properties = new System.Collections.Generic.Dictionary<string, Puerts.PropertyRegisterInfo>()
                {
                    
                    {"DMF", new Puerts.PropertyRegisterInfo(){ IsStatic = false, Getter = G_DMF, Setter = S_DMF} },

                    {"DSF", new Puerts.PropertyRegisterInfo(){ IsStatic = true, Getter = G_DSF, Setter = S_DSF} },

                    {"MyCallback", new Puerts.PropertyRegisterInfo(){ IsStatic = false, Getter = G_MyCallback, Setter = S_MyCallback} }
                },
                LazyMembers = new System.Collections.Generic.List<Puerts.LazyMemberRegisterInfo>()
                {   
                }
            };
        }
    
    }
}
