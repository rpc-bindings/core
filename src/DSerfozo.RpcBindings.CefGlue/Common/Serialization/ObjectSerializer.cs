using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class ObjectSerializer
    {
        public static CefValue Serialize(object obj, HashSet<object> seen, int? index = null, string key = null)
        {
            var result = CefValue.Create();
            result.SetNull();

            if (obj != null)
            {
                if (seen.Contains(obj))
                {
                    return result;
                }
                seen.Add(obj);
            }

            var type = obj?.GetType();

            if (type == typeof(string))
            {
                result.SetString(obj as string, index, key);
            }
            else if (type == typeof(char))
            {
                result.SetString(new string((char) obj, 1), index, key);
            }
            else if (type == typeof(byte))
            {
                result.SetInt((byte) obj, index, key);
            }
            else if (type == typeof(sbyte))
            {
                result.SetInt((sbyte) obj, index, key);
            }
            else if (type == typeof(short))
            {
                result.SetInt((short) obj, index, key);
            }
            else if (type == typeof(ushort))
            {
                result.SetInt((ushort) obj, index, key);
            }
            else if (type == typeof(int))
            {
                result.SetInt((int) obj, index, key);
            }
            else if (type == typeof(uint))
            {
                result.SetDouble((uint) obj, index, key);
            }
            else if (type == typeof(long))
            {
                result.SetInt64((long) obj, index, key);
            }
            else if (type == typeof(ulong))
            {
                result.SetDouble((ulong) obj, index, key);
            }
            else if (type == typeof(float))
            {
                result.SetDouble((float) obj, index, key);
            }
            else if (type == typeof(double))
            {
                result.SetDouble((double) obj, index, key);
            }
            else if (type == typeof(decimal))
            {
                result.SetDouble(Convert.ToDouble((decimal) obj), index, key);
            }
            else if (type == typeof(bool))
            {
                result.SetBool((bool) obj, index, key);
            }
            else if (type == typeof(DateTime))
            {
                result.SetTime((DateTime) obj, index, key);
            }
            else if (type?.IsArray == true)
            {
                var array = (Array) obj;
                using (var value = CefListValue.Create())
                {
                    value.SetInt(0, (byte) CefTypes.Array);

                    for (var i = 1; i < array.Length + 1; i++)
                    {
                        value.SetValue(i, Serialize(array.GetValue(i - 1), seen, i));
                    }

                    result.SetList(value);
                }
            }
            else if (type?.IsPrimitive == false && !type.IsEnum)
            {
                using (var value = CefDictionaryValue.Create())
                {
                    var properties = type.GetProperties();

                    foreach (var property in properties)
                    {
                        var propertyValue = property.GetValue(obj);
                        var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
                        var name = dataMember?.Name ?? property.Name;

                        value.SetValue(name, Serialize(propertyValue, seen));
                    }

                    result.SetDictionary(value);
                }
            }

            return result;
        }

        public static object Deserialize(CefValue value, Type targetType, int? index = null, string key = null)
        {
            var type = value.GetValueType();
            object result = null;

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            switch (type)
            {
                case CefValueType.Invalid:
                case CefValueType.Null:
                    break;
                case CefValueType.Binary:
                    if (value.IsType(CefTypes.Time) && targetType == typeof(DateTime))
                    {
                        result = value.GetTime();
                    }
                    break;
                case CefValueType.Bool:
                    if (targetType == typeof(bool) || targetType == typeof(object))
                    {
                        result = value.GetBool();
                    }
                    break;
                case CefValueType.Int:
                    var intVal = value.GetInt();
                    if (targetType == typeof(byte))
                    {
                        result = Convert.ToByte(intVal);
                    }
                    else if (targetType == typeof(sbyte))
                    {
                        result = Convert.ToSByte(intVal);
                    }
                    else if (targetType == typeof(short))
                    {
                        result = Convert.ToInt16(intVal);
                    }
                    else if (targetType == typeof(ushort))
                    {
                        result = Convert.ToUInt16(intVal);
                    }
                    else if (targetType == typeof(int) || targetType == typeof(object))
                    {
                        result = intVal;
                    }
                    else if (targetType == typeof(uint))
                    {
                        result = Convert.ToUInt32(intVal);
                    }
                    else if (targetType == typeof(long))
                    {
                        result = Convert.ToInt64(intVal);
                    }
                    else if (targetType == typeof(ulong))
                    {
                        result = Convert.ToUInt64(intVal);
                    }
                    else if (targetType == typeof(double))
                    {
                        result = Convert.ToDouble(intVal);
                    }
                    else if (targetType == typeof(float))
                    {
                        result = Convert.ToSingle(intVal);
                    }
                    else if (targetType == typeof(decimal))
                    {
                        result = Convert.ToDecimal(intVal);
                    }
                    break;
                case CefValueType.Double:
                    var doubleVal = value.GetDouble();
                    if (targetType == typeof(byte))
                    {
                        result = Convert.ToByte(doubleVal);
                    }
                    else if (targetType == typeof(sbyte))
                    {
                        result = Convert.ToSByte(doubleVal);
                    }
                    else if (targetType == typeof(short))
                    {
                        result = Convert.ToInt16(doubleVal);
                    }
                    else if (targetType == typeof(ushort))
                    {
                        result = Convert.ToUInt16(doubleVal);
                    }
                    else if (targetType == typeof(int))
                    {
                        result = Convert.ToInt32(doubleVal);
                    }
                    else if (targetType == typeof(uint))
                    {
                        result = Convert.ToUInt32(doubleVal);
                    }
                    else if (targetType == typeof(long))
                    {
                        result = Convert.ToInt64(doubleVal);
                    }
                    else if (targetType == typeof(ulong))
                    {
                        result = Convert.ToUInt64(doubleVal);
                    }
                    else if (targetType == typeof(double) || targetType == typeof(object))
                    {
                        result = doubleVal;
                    }
                    else if (targetType == typeof(float))
                    {
                        result = Convert.ToSingle(doubleVal);
                    }
                    else if (targetType == typeof(decimal))
                    {
                        result = Convert.ToDecimal(doubleVal);
                    }
                    break;
                case CefValueType.String:
                    var strVal = value.GetString();
                    if (targetType == typeof(string) || targetType == typeof(object))
                    {
                        result = strVal;
                    }
                    else if (targetType == typeof(char) && !string.IsNullOrEmpty(strVal))
                    {
                        result = strVal.First();
                    }
                    break;
                case CefValueType.Dictionary:
                    using (var dictVal = value.GetDictionary())
                    {
                        try
                        {
                            result = Activator.CreateInstance(targetType);
                            var properties = targetType.GetProperties()
                                .Select(p => new { Prop = p, DataMember = p.GetCustomAttribute<DataMemberAttribute>() })
                                .ToDictionary(k => k.DataMember?.Name ?? k.Prop.Name, v => v.Prop);
                            var keys = dictVal.GetKeys();
                            foreach (var dictKey in keys)
                            {
                                if (properties.TryGetValue(dictKey, out var matchingProperty))
                                {
                                    matchingProperty.SetValue(result,
                                        Deserialize(dictVal.GetValue(dictKey), matchingProperty.PropertyType));
                                }
                            }
                        }
                        catch
                        {
                            
                        }
                    }
                    break;
                case CefValueType.List:
                    using (var lstVal = value.GetList())
                    {
                        var cefType = (CefTypes) lstVal.GetInt(0);
                        if (targetType.IsArray && cefType == CefTypes.Array)
                        {
                            var elementType = targetType.GetElementType();
                            var array = Activator.CreateInstance(targetType, new object[]{lstVal.Count - 1}) as Array;

                            for (var i = 0; i < lstVal.Count - 1; i++)
                            {
                                array.SetValue(Deserialize(lstVal.GetValue(i + 1), elementType), i);
                            }

                            result = array;
                        }
                    }
                    break;
            }
            return result;
        }

        private static void SetString(this CefValue @this, string value, int? index, string key)
        {
            var type = @this.GetValueType();
            switch (type)
            {
                case CefValueType.List:
                    using (var listValue = @this.GetList())
                    {
                        if (index.HasValue)
                        {
                            listValue.SetString(index.Value, value);
                        }
                    }
                    break;
                case CefValueType.Dictionary:
                    using (var dictValue = @this.GetDictionary())
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            dictValue.SetString(key, value);
                        }
                    }
                    break;
                default:
                    @this.SetString(value);
                    break;
            }
        }

        private static void SetInt(this CefValue @this, int value, int? index, string key)
        {
            var type = @this.GetValueType();
            switch (type)
            {
                case CefValueType.List:
                    using (var listValue = @this.GetList())
                    {
                        if (index.HasValue)
                        {
                            listValue.SetInt(index.Value, value);
                        }
                    }
                    break;
                case CefValueType.Dictionary:
                    using (var dictValue = @this.GetDictionary())
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            dictValue.SetInt(key, value);
                        }
                    }
                    break;
                default:
                    @this.SetInt(value);
                    break;
            }
        }

        private static void SetDouble(this CefValue @this, double value, int? index, string key)
        {
            var type = @this.GetValueType();
            switch (type)
            {
                case CefValueType.List:
                    using (var listValue = @this.GetList())
                    {
                        if (index.HasValue)
                        {
                            listValue.SetDouble(index.Value, value);
                        }
                    }
                    break;
                case CefValueType.Dictionary:
                    using (var dictValue = @this.GetDictionary())
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            dictValue.SetDouble(key, value);
                        }
                    }
                    break;
                default:
                    @this.SetDouble(value);
                    break;
            }
        }

        private static void SetBool(this CefValue @this, bool value, int? index, string key)
        {
            var type = @this.GetValueType();
            switch (type)
            {
                case CefValueType.List:
                    using (var listValue = @this.GetList())
                    {
                        if (index.HasValue)
                        {
                            listValue.SetBool(index.Value, value);
                        }
                    }
                    break;
                case CefValueType.Dictionary:
                    using (var dictValue = @this.GetDictionary())
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            dictValue.SetBool(key, value);
                        }
                    }
                    break;
                default:
                    @this.SetBool(value);
                    break;
            }
        }

        private static void SetTime(this CefValue @this, DateTime value, int? index, string key)
        {
            var type = @this.GetValueType();
            using (var val = CefValue.Create())
            {
                val.SetTime(value);
                switch (type)
                {
                    case CefValueType.List:
                        using (var listValue = @this.GetList())
                        {
                            if (index.HasValue)
                            {
                                listValue.SetValue(index.Value, val);
                            }
                        }
                        break;
                    case CefValueType.Dictionary:
                        using (var dictValue = @this.GetDictionary())
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                dictValue.SetValue(key, val);
                            }
                        }
                        break;
                    default:
                        @this.SetTime(value);
                        break;
                }
            }
        }

        private static void SetInt64(this CefValue @this, long value, int? index, string key)
        {
            var type = @this.GetValueType();
            using (var val = CefValue.Create())
            {
                val.SetInt64(value);
                switch (type)
                {
                    case CefValueType.List:
                        using (var listValue = @this.GetList())
                        {
                            if (index.HasValue)
                            {
                                listValue.SetValue(index.Value, val);
                            }
                        }
                        break;
                    case CefValueType.Dictionary:
                        using (var dictValue = @this.GetDictionary())
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                dictValue.SetValue(key, val);
                            }
                        }
                        break;
                    default:
                        @this.SetInt64(value);
                        break;
                }
            }
        }
    }
}
