using DSerfozo.RpcBindings.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Marshaling.Delegates
{
    public class CallbackDelegateGenerator<TMarshal>
    {
        private const string DelegateMethodName = "DynamicCall";

        private readonly IDictionary<Type, Type> typeCache = new Dictionary<Type, Type>();

        public Delegate Generate(Type delegateType, int id, ICallbackExecutor<TMarshal> callbackExecutor, IParameterBinder<TMarshal> parameterBinder)
        {
            ValidateDelegateType(delegateType);

            if(!typeCache.TryGetValue(delegateType, out var delegateContainerType))
            {
                delegateContainerType = GenerateDelegateContainerType(delegateType);
                typeCache.Add(delegateType, delegateContainerType);
            }

            var created = Activator.CreateInstance(delegateContainerType, new object[] { id, callbackExecutor, parameterBinder });

            return Delegate.CreateDelegate(delegateType, created, DelegateMethodName, false, true); ;
        }

        private static Type GenerateDelegateContainerType(Type delegateType)
        {
            var methodInfo = delegateType.GetMethod(nameof(Action.Invoke));
            var argumentTypes = methodInfo.GetParameters().Select(s => s.ParameterType).ToArray();
            var returnType = methodInfo.ReturnType;

            var assemblyName = new AssemblyName("AssemblyName");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(assemblyName.FullName
                              , TypeAttributes.Public |
                              TypeAttributes.Class |
                              TypeAttributes.AutoClass |
                              TypeAttributes.AnsiClass |
                              TypeAttributes.BeforeFieldInit
                              , null);
            typeBuilder.SetParent(typeof(DelegateContainerBase<TMarshal>));
            var parentConstructor = typeof(DelegateContainerBase<TMarshal>).GetConstructor(new[] { typeof(int), typeof(ICallbackExecutor<TMarshal>), typeof(IParameterBinder<TMarshal>) });
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, CallingConventions.Standard |
                CallingConventions.HasThis, new[] { typeof(int), typeof(ICallbackExecutor<TMarshal>), typeof(IParameterBinder<TMarshal>) });
            var constructorGenerator = constructorBuilder.GetILGenerator();
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            constructorGenerator.Emit(OpCodes.Ldarg_3);
            constructorGenerator.Emit(OpCodes.Call, parentConstructor);
            constructorGenerator.Emit(OpCodes.Ret);
            var parentMethod = typeof(DelegateContainerBase<TMarshal>).GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            var executeMethod = parentMethod.MakeGenericMethod(returnType.GenericTypeArguments.First());
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
            if(!typeof(Task).IsAssignableFrom(methodInfo.ReturnType) ||
                !methodInfo.ReturnType.IsGenericType)
            {
                throw new InvalidOperationException("The supplied delegate type must have a Task<T> return value.");
            }
        }
    }
}
