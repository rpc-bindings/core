﻿using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Execution
{
    public class PropertyExecutor<TMarshal> : IPropertyExecutor<TMarshal>
    {
        private readonly IReadOnlyDictionary<int, ObjectDescriptor> objects;
        private readonly IParameterBinder<TMarshal> parameterBinder;

        public PropertyExecutor(IReadOnlyDictionary<int, ObjectDescriptor> objects, IParameterBinder<TMarshal> parameterBinder)
        {
            this.objects = objects;
            this.parameterBinder = parameterBinder;
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
                result.Value = parameterBinder.BindToWire(getResult);
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
                propertyDescriptor.Setter(objectDescriptor.Object, parameterBinder.BindToNet(
                    new ParameterBinding<TMarshal>
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