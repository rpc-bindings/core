using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Execution;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Execution
{
    public class PropertyExecutorTests
    {
        [Fact]
        public void ThrowsForNonExistentObjectWhenGet()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var propertyExecutor = new PropertyExecutor<object>(
                    new ReadOnlyDictionary<long, ObjectDescriptor>(new Dictionary<long, ObjectDescriptor>()),
                    context => { });
                propertyExecutor.Execute(new PropertyGetExecution()
                {
                    ObjectId = 1
                });
            });
        }

        [Fact]
        public void ThrowsForNonExistentObjectWhenSet()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var propertyExecutor = new PropertyExecutor<object>(
                    new ReadOnlyDictionary<long, ObjectDescriptor>(new Dictionary<long, ObjectDescriptor>()),
                    context => { });
                propertyExecutor.Execute(new PropertySetExecution<object>()
                {
                    ObjectId = 1
                });
            });
        }

        [Fact]
        public void ThrowsForNonExistentPropertyWhenGet()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var propertyExecutor = new PropertyExecutor<object>(
                    new ReadOnlyDictionary<long, ObjectDescriptor>(
                        new Dictionary<long, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>()).WithId(1).Get() }
                        }), context => { });
                propertyExecutor.Execute(new PropertyGetExecution()
                {
                    ObjectId = 1,
                    PropertyId = 2
                });
            });
        }

        [Fact]
        public void ThrowsForNonExistentPropertyWhenSet()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var propertyExecutor = new PropertyExecutor<object>(
                    new ReadOnlyDictionary<long, ObjectDescriptor>(
                        new Dictionary<long, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>()).WithId(1).Get() }
                        }), context => { });
                propertyExecutor.Execute(new PropertySetExecution<object>()
                {
                    ObjectId = 1,
                    PropertyId = 2
                });
            });
        }


        [Fact]
        public void GetExecptionPassedAlong()
        {
            const string message = "message";

            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        { 1, ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>(){ PropertyDescriptor.Create().WithId(2).WithGetter(o => throw new Exception(message)).Get() }).WithId(1).Get() }
                    }), context => { });

            var result = propertyExecutor.Execute(new PropertyGetExecution
            {
                ObjectId = 1,
                PropertyId = 2
            });

            Assert.False(result.Success);
            Assert.Equal(message, result.Error);
        }

        [Fact]
        public void SetExecptionPassedAlong()
        {
            const string message = "message";

            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        { 1, ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>(){ PropertyDescriptor.Create().WithId(2).WithSetter((o, o1) => throw new Exception(message)).Get() }).WithId(1).Get() }
                    }), context => { });

            var result = propertyExecutor.Execute(new PropertySetExecution<object>
            {
                ObjectId = 1,
                PropertyId = 2
            });

            Assert.False(result.Success);
            Assert.Equal(message, result.Error);
        }

        [Fact]
        public void ExecutionIdMatchesForGet()
        {
            const string message = "message";

            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        { 1, ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>(){ PropertyDescriptor.Create().WithId(2).WithGetter(o => throw new Exception(message)).Get() }).WithId(1).Get() }
                    }), context => { });

            var result = propertyExecutor.Execute(new PropertyGetExecution
            {
                ObjectId = 1,
                PropertyId = 2,
                ExecutionId = 3
            });

            Assert.Equal(3, result.ExecutionId);
        }

        [Fact]
        public void ExecutionIdMatchesForSet()
        {
            const string message = "message";

            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        {
                            1,
                            ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>()
                            {
                                PropertyDescriptor.Create().WithId(2).WithSetter((_, __) => throw new Exception(message))
                                    .Get()
                            }).WithId(1).Get()
                        }
                    }), context => { });

            var result = propertyExecutor.Execute(new PropertySetExecution<object>
            {
                ObjectId = 1,
                PropertyId = 2,
                ExecutionId = 3
            });

            Assert.Equal(3, result.ExecutionId);
        }

        [Fact]
        public void GetDone()
        {
            const string message = "message";

            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        {
                            1,
                            ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>()
                            {
                                PropertyDescriptor.Create().WithId(2).WithGetter(o => message)
                                    .Get()
                            }).WithId(1).Get()
                        }
                    }), context => {
                    if (context.Direction == ObjectBindingDirection.In)
                        context.ObjectValue = context.NativeValue;
                    else
                        context.NativeValue = context.ObjectValue;
                });

            var result = propertyExecutor.Execute(new PropertyGetExecution
            {
                ObjectId = 1,
                PropertyId = 2,
                ExecutionId = 3
            });

            Assert.Equal(message, result.Value);
        }

        [Fact]
        public void SetDone()
        {
            const string message = "message";

            var setted = string.Empty;
            var propertyExecutor = new PropertyExecutor<object>(
                new ReadOnlyDictionary<long, ObjectDescriptor>(
                    new Dictionary<long, ObjectDescriptor>()
                    {
                        {
                            1,
                            ObjectDescriptor.Create().WithProperties(new List<PropertyDescriptor>()
                            {
                                PropertyDescriptor.Create().WithId(2).WithSetter((_, __) => setted = (string)__)
                                    .Get()
                            }).WithId(1).Get()
                        }
                    }), context => {
                    if (context.Direction == ObjectBindingDirection.In)
                        context.ObjectValue = context.NativeValue;
                    else
                        context.NativeValue = context.ObjectValue;
                });

            var result = propertyExecutor.Execute(new PropertySetExecution<object>
            {
                ObjectId = 1,
                PropertyId = 2,
                ExecutionId = 3,
                Value = message
            });

            Assert.Equal(message, setted);
        }
    }
}
