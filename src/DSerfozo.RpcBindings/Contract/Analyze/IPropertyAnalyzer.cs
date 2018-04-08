using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract.Analyze
{
    public interface IPropertyAnalyzer
    {
        IEnumerable<PropertyDescriptor> AnalyzeProperties(Type type, object obj, bool extractValue);
    }
}