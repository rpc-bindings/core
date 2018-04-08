using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Binding;
using DSerfozo.RpcBindings.CefGlue.Renderer.Services;
using DSerfozo.RpcBindings.CefGlue.Renderer.Util;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Serialization
{
    public class V8Serializer
    {
        private readonly PromiseService promiseService;
        private readonly SavedValueRegistry<Callback> callbackRegistry;
        private readonly SavedValueFactory<Promise> functionCallPromiseRegistry;

        public V8Serializer(PromiseService promiseService, SavedValueRegistry<Callback> callbackRegistry,
            SavedValueFactory<Promise> functionCallPromiseRegistry)
        {
            this.promiseService = promiseService;
            this.callbackRegistry = callbackRegistry;
            this.functionCallPromiseRegistry = functionCallPromiseRegistry;
        }

        public CefValue Serialize(CefV8Value value)
        {
            var list = new List<CefV8Value>();
            var result = Serialize(value, callbackRegistry, promiseService, list);
            list.Where(f => !f.IsFunction).ToList().ForEach(f => f.Dispose());
            return result;
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
                using (var list = CefListValue.Create())
                {
                    list.SetSize(value.GetArrayLength());
                    for (var i = 0; i < value.GetArrayLength(); i++)
                    {
                        list.SetValue(i, Serialize(value.GetValue(i), callbackRegistry, promiseService, seen));
                    }
                    result.SetList(list);
                }
            }
            else if (value.IsFunction)
            {
                var callback = new Callback(value, promiseService, this);
                var id = callbackRegistry.Save(frameId, callback);

                using (var list = CefDictionaryValue.Create())
                    using(var actualValue = CefDictionaryValue.Create() )
                {
                    list.SetString(ObjectSerializer.TypeIdPropertyName, CallbackDescriptor.TypeId);

                    actualValue.SetInt64(nameof(CallbackDescriptor.FunctionId), id);

                    list.SetDictionary(ObjectSerializer.ValuePropertyName, actualValue);
                    result.SetDictionary(list);
                }
            }
            else if (value.IsObject)
            {
                using (var dict = CefDictionaryValue.Create())
                using(var actualValue = CefDictionaryValue.Create())
                {
                    dict.SetString(ObjectSerializer.TypeIdPropertyName, ObjectSerializer.DictionaryTypeId);
                    if (value.TryGetKeys(out var keys))
                    {
                        foreach (var key in keys)
                        {
                            actualValue.SetValue(key, Serialize(value.GetValue(key), callbackRegistry, promiseService, seen));
                        }
                    }

                    dict.SetDictionary(ObjectSerializer.ValuePropertyName, actualValue);
                    result.SetDictionary(dict);
                }
            }

            return result;
        }

        public CefV8Value Deserialize(CefValue value)
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
                        var array = CefV8Value.CreateArray(list.Count);
                        for (var i = 0; i < list.Count; i++)
                        {
                            using (var cefValue = list.GetValue(i))
                            {
                                array.SetValue(i, Deserialize(cefValue));
                            }
                        }

                        return array;
                    }
                }
            }
            if (valueType == CefValueType.Dictionary)
            {
                using (var dict = value.GetDictionary())
                {
                    var typeId = dict.GetString(ObjectSerializer.TypeIdPropertyName);
                    using (var actualValue = dict.GetDictionary(ObjectSerializer.ValuePropertyName))
                    {
                        if (typeId == ObjectSerializer.DictionaryTypeId)
                        {
                            var obj = CefV8Value.CreateObject();
                            foreach (var key in actualValue.GetKeys())
                            {
                                obj.SetValue(key, Deserialize(actualValue.GetValue(key)));
                            }
                            return obj;
                        }

                        if (typeId == ObjectDescriptor.TypeId)
                        {
                            var descriptor = ObjectDescriptorSerializer.ReadObjectDescriptor(actualValue, this);
                            return new ObjectBinder(descriptor, this, functionCallPromiseRegistry).BindToNew();
                        }
                    }
                }
            }
            

            return CefV8Value.CreateNull();
        }

    }
}
