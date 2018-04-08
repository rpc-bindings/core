using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Castle.DynamicProxy;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Marshaling.Delegates
{
    public class CallbackDelegateGenerator<TMarshal>
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private const string DelegateMethodName = "DynamicCall";

        private readonly IDictionary<Type, Type> typeCache = new Dictionary<Type, Type>();

        public Delegate Generate(Type delegateType, long id, ICallbackExecutor<TMarshal> callbackExecutor, BindingDelegate<TMarshal> parameterBinder)
        {
            ValidateDelegateType(delegateType);

            if(!typeCache.TryGetValue(delegateType, out var delegateContainerType))
            {
                delegateContainerType = GenerateDelegateContainerType(delegateType);
                typeCache.Add(delegateType, delegateContainerType);
            }

            var created = Activator.CreateInstance(delegateContainerType, id, callbackExecutor, parameterBinder, delegateType);

            return Delegate.CreateDelegate(delegateType, created, DelegateMethodName, false, true); ;
        }

        private static Type GenerateDelegateContainerType(Type delegateType)
        {
            var methodInfo = delegateType.GetMethod(nameof(Action.Invoke));
            var parameterInfos = methodInfo.GetParameters();
            var argumentTypes = parameterInfos.Select(s => s.ParameterType).ToArray();
            var returnType = methodInfo.ReturnType;
            var underlyingReturnType = typeof(object);
            if (returnType.IsGenericType)
            {
                underlyingReturnType = returnType.GetGenericArguments().First();
            }

            var moduleScope = Generator.ProxyBuilder.ModuleScope;
            var moduleBuilder = moduleScope.ObtainDynamicModuleWithStrongName();
            var targetIntfName =
                "Castle.Proxies.Delegates." +
                delegateType.ToString()
                    .Replace('.', '_')
                    .Replace(',', '`')
                    .Replace("+", "__")
                    .Replace("[", "``")
                    .Replace("]", "``");
            var typeName = moduleScope.NamingScope.GetUniqueName(targetIntfName);
            var typeBuilder = moduleBuilder.DefineType(typeName
                              , TypeAttributes.Public |
                              TypeAttributes.Class |
                              TypeAttributes.AutoClass |
                              TypeAttributes.AnsiClass |
                              TypeAttributes.BeforeFieldInit
                              , null);
            typeBuilder.SetParent(typeof(DelegateContainerBase<TMarshal>));
            var type = typeof(DelegateContainerBase<TMarshal>);
            var parentConstructor = type.GetConstructor(new[] { typeof(long), typeof(ICallbackExecutor<TMarshal>), typeof(BindingDelegate<TMarshal>), typeof(Type), typeof(Type) });
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, CallingConventions.Standard |
                CallingConventions.HasThis, new[] { typeof(long), typeof(ICallbackExecutor<TMarshal>), typeof(BindingDelegate<TMarshal>), typeof(Type) });
            var constructorGenerator = constructorBuilder.GetILGenerator();
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            constructorGenerator.Emit(OpCodes.Ldarg_3);
            constructorGenerator.Emit(OpCodes.Ldtoken, underlyingReturnType);
            constructorGenerator.Emit(OpCodes.Call,
                typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Static | BindingFlags.Public));
            constructorGenerator.Emit(OpCodes.Ldarg_S, 4);
            constructorGenerator.Emit(OpCodes.Call, parentConstructor);
            constructorGenerator.Emit(OpCodes.Ret);
            var parentMethod = typeof(DelegateContainerBase<TMarshal>).GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            var executeMethod = parentMethod.MakeGenericMethod(underlyingReturnType);
            var methodBuilder = typeBuilder.DefineMethod(DelegateMethodName, MethodAttributes.Public |
                MethodAttributes.HideBySig, returnType, argumentTypes);
            var methodGenerator = methodBuilder.GetILGenerator();
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldc_I4_S, argumentTypes.Length);
            methodGenerator.Emit(OpCodes.Newarr, typeof(object));

            for (int i = 0; i < argumentTypes.Length; i++)
            {
                var argumentType = argumentTypes[i];
                methodGenerator.Emit(OpCodes.Dup);
                methodGenerator.Emit(OpCodes.Ldc_I4_S, i);
                methodGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                if (argumentType.IsValueType)
                {
                    methodGenerator.Emit(OpCodes.Box);
                }
                methodGenerator.Emit(OpCodes.Stelem_Ref);
            }
            methodGenerator.Emit(OpCodes.Call, executeMethod);
            methodGenerator.Emit(OpCodes.Ret);

            return typeBuilder.CreateType();
        }

        private static void ValidateDelegateType(Type delegateType)
        {
            var methodInfo = delegateType.GetMethod(nameof(Action.Invoke));
            if(!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                throw new InvalidOperationException("The supplied delegate type must have a Task return value.");
            }
        }
    }
}
