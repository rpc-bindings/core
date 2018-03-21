using System;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Marshaling;

namespace DSerfozo.RpcBindings.Extensions
{
    public static class ObjectBinderBuilderExtensions
    {
        private const string BindMethodName = "Bind";

        public static IObjectBinderBuilder<TMarshal> Use<TMarshal>(this IObjectBinderBuilder<TMarshal> @this,
            Type binderType, params object[] args)
        {
            return @this.Use(nextBinder =>
            {
                var bindMethod = binderType.GetMethod(BindMethodName);
                if (bindMethod == null)
                {
                    throw new InvalidOperationException("Missing Bind method");
                }

                var parameters = bindMethod.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(BindingContext<TMarshal>))
                {
                    throw new InvalidOperationException("Bind method should have a single parameter of BindingContext.");
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = nextBinder;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);

                var binder = Activator.CreateInstance(binderType, ctorArgs);
                return (BindingDelegate<TMarshal>)bindMethod.CreateDelegate(typeof(BindingDelegate<TMarshal>), binder);
            });
        }
    }
}