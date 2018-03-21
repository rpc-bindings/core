using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Extensions;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Execution
{
    public class PropertyExecutor<TMarshal> : IPropertyExecutor<TMarshal>, IBinder<TMarshal>
    {
        private readonly IReadOnlyDictionary<long, ObjectDescriptor> objects;
        private readonly BindingDelegate<TMarshal> binder;

        BindingDelegate<TMarshal> IBinder<TMarshal>.Binder => binder;

        public PropertyExecutor(IReadOnlyDictionary<long, ObjectDescriptor> objects, BindingDelegate<TMarshal> binder)
        {
            this.objects = objects;
            this.binder = binder;
        }

        public PropertyGetSetResult<TMarshal> Execute(PropertyGetExecution propertyGetExecution)
        {
            GetPropertyDescriptor(propertyGetExecution.ObjectId, propertyGetExecution.PropertyId, out var propertyDescriptor, out var objectDescriptor);

            var result = new PropertyGetSetResult<TMarshal>
            {
                ExecutionId = propertyGetExecution.ExecutionId
            };
            try
            {
                var getResult = propertyDescriptor.Getter(objectDescriptor.Object);
                result.Value = this.BindToWire(getResult);
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        public PropertyGetSetResult<TMarshal> Execute(PropertySetExecution<TMarshal> propertySetExecution)
        {
            GetPropertyDescriptor(propertySetExecution.ObjectId, propertySetExecution.PropertyId, out var propertyDescriptor, out var objectDescriptor);

            var result = new PropertyGetSetResult<TMarshal>
            {
                ExecutionId = propertySetExecution.ExecutionId
            };
            try
            {
                propertyDescriptor.Setter(objectDescriptor.Object, this.BindToNet(
                    new Binding<TMarshal>
                    {
                        Value = propertySetExecution.Value,
                        TargetType = propertyDescriptor.Type
                    }));
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        private void GetPropertyDescriptor(int objectId, int propertyId, out PropertyDescriptor propertyDescriptor, out ObjectDescriptor objectDescriptor)
        {
            if (!objects.TryGetValue(objectId, out objectDescriptor))
            {
                throw new InvalidOperationException("");
            }

            if (!objectDescriptor.Properties.TryGetValue(propertyId, out propertyDescriptor))
            {
                throw new InvalidOperationException("");
            }
        }
    }
}
