using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IMethodAnalyzer
    {
        IEnumerable<MethodDescriptor> AnalyzeMethods(Type type);
    }
}