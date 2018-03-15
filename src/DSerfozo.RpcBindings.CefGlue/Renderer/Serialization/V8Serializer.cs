using System;
using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Services;
using DSerfozo.RpcBindings.CefGlue.Renderer.Util;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Serialization
{
    public class V8Serializer
    {
        private readonly PromiseService promiseService;
        private readonly SavedValueRegistry<Callback> callbackRegistry;

        public V8Serializer(PromiseService promiseService, SavedValueRegistry<Callback> callbackRegistry)
        {
            this.promiseService = promiseService;
            this.callbackRegistry = callbackRegistry;
        }

        public CefValue Serialize(CefV8Value value)
        {
            var list = new List<CefV8Value>();
            var result = Serialize(value, callbackRegistry, promiseService, list);
            list.Where(f => !f.IsFunction).ToList().ForEach(f => f.Dispose());
            return result;
        }

        public CefV8Value Deserialize(CefValue value)
        {
            return Deserialize(value, false);
        }

        public CefValue Serialize(CefV8Value value, SavedValueRegistry<Callback> callbackRegistry, PromiseService promiseService, List<CefV8Value> seen)
        {
            long frameId = 0;
            using (var context = CefV8Context.GetCurrentContext())
            {
                frameId = context.GetFrame().Identifier;
            }
            var result = CefValue.Create();
            result.SetNull();

            if (seen.Any(s => s.IsSame(value)))
            {
                return result;
            }
            else
            {
                seen.Add(value);
            }

            if (value.IsString)
            {
                result.SetString(value.GetStringValue());
            }
            else if (value.IsBool)
            {
                result.SetBool(value.GetBoolValue());
            }
            else if (value.IsDouble)
            {
                result.SetDouble(value.GetDoubleValue());
            }
            else if (value.IsInt)
            {
                result.SetInt(value.GetIntValue());
            }
            else if (value.IsUInt)
            {
                result.SetDouble(value.GetUIntValue());
            }
            else if (value.IsDate)
            {
                result.SetTime(value.GetDateValue());
            }
            else if (value.IsArray)
            {
                /*using (*/
                var list = CefListValue.Create() /*)*/;
                {
                    list.SetSize(value.GetArrayLength() + 1);
                    list.SetInt(0, (int) CefTypes.Array);
                    for (var i = 0; i < value.GetArrayLength(); i++)
                    {
                        list.SetValue(i + 1, Serialize(value.GetValue(i), callbackRegistry, promiseService, seen));
                    }
                    result.SetList(list);
                }
            }
            else if (value.IsFunction)
            {
                using (var list = CefListValue.Create())
                {
                    var callback = new Callback(value, promiseService, this);
                    var id = callbackRegistry.Save(frameId, callback);

                    list.SetInt(0, (int)CefTypes.Callback);
                    list.SetInt(1, id);

                    result.SetList(list);
                }
            }
            else if (value.IsObject)
            {
                using (var dict = CefDictionaryValue.Create())
                {
                    if (value.TryGetKeys(out var keys))
                    {
                        foreach (var key in keys)
                        {
                            dict.SetValue(key, Serialize(value.GetValue(key), callbackRegistry, promiseService, seen));
                        }
                    }

                    result.SetDictionary(dict);
                }
            }

            return result;
        }

        public static CefV8Value Deserialize(CefValue value, bool t = false)
        {
            var valueType = value.GetValueType();

            if (valueType == CefValueType.String)
            {
                return CefV8Value.CreateString(value.GetString());
            }
            if (valueType == CefValueType.Int)
            {
                return CefV8Value.CreateInt(value.GetInt());
            }
            if (valueType == CefValueType.Double)
            {
                return CefV8Value.CreateDouble(value.GetDouble());
            }
            if (value.IsType(CefTypes.Int64))
            {
                return CefV8Value.CreateDouble(value.GetInt64());
            }
            if(value.IsType(CefTypes.Time))
            {
                return CefV8Value.CreateDate(value.GetTime());
            }
            if (valueType == CefValueType.Bool)
            {
                return CefV8Value.CreateBool(value.GetBool());
            }
            if (valueType == CefValueType.List)
            {
                using (var list = value.GetList())
                {
                    if (list.Count > 0)
                    {
                        var type = (CefTypes)list.GetInt(0);
                        if (type == CefTypes.Array)
                        {
                            var array = CefV8Value.CreateArray(list.Count - 1);
                            for (var i = 0; i < list.Count - 1; i++)
                            {
                                array.SetValue(i, Deserialize(list.GetValue(i + 1), t));
                            }

                            return array;
                        }
                    }
                }
            }
            if (valueType == CefValueType.Dictionary)
            {
                using (var dict = value.GetDictionary())
                {
                    var obj = CefV8Value.CreateObject();
                    foreach (var key in dict.GetKeys())
                    {
                        obj.SetValue(key, Deserialize(dict.GetValue(key), t));
                    }
                    return obj;
                }
            }
            

            return CefV8Value.CreateNull();
        }

    }
}
