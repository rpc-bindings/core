using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using DSerfozo.RpcBindings.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Execution;
using DSerfozo.RpcBindings.Execution.Model;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Calling
{
    public class MethodExecutorTests
    {
        [Fact]
        public Task ThrowsForNonExistentObject()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(async () => 
            {
                var methodExecutor = new MethodExecutor<object>(new ReadOnlyDictionary<int, ObjectDescriptor>(new Dictionary<int, ObjectDescriptor>()), new NoopObjectParameterBinder());
                await methodExecutor.Execute(new MethodExecution<object>()
                {
                    ObjectId = 1
                });
            });
        }

        [Fact]
        public Task ThrowsForNonExistentMethod()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());
                await methodExecutor.Execute(new MethodExecution<object>()
                {
                    ObjectId = 1,
                    MethodId = 2
                });
            });
        }

        [Fact]
        public async Task MethodParameterCountMismatchThrows()
        {
            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()
                            {
                                MethodDescriptor.Create().WithId(2).WithParameterCount(2).Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { }
            });

            Assert.Equal("Parameter mismatch.", result.Error);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task MethodParameterTypeMismatchThrows()
        {
            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()
                            {
                                MethodDescriptor.Create().WithId(2).WithParameterCount(1).WithParameters(new List<MethodParameterDescriptor>
                                {
                                    new MethodParameterDescriptor(typeof(string), false)
                                }).Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { 1 }
            });

            Assert.Equal("Parameter mismatch.", result.Error);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task MethodCalled()
        {
            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()
                            {
                                MethodDescriptor.Create()
                                .WithId(2)
                                .WithParameterCount(1)
                                .WithParameters(new List<MethodParameterDescriptor>
                                {
                                    new MethodParameterDescriptor(typeof(string), false)
                                })
                                .WithExecute((o, a) => a[0] as string)
                                .Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            const string Value = "expected";
            const int ExecutionId = 3;
            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ExecutionId = ExecutionId,
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { Value }
            });

            Assert.Equal(Value, result.Result as string);
            Assert.Equal(ExecutionId, result.ExecutionId);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task MethodCallExceptionPassedAlong()
        {
            var method = typeof(SimpleClassWithExceptions).GetMethod("ThrowException");

            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>
                        {
                            { 1, ObjectDescriptor.Create()
                            .WithObject(new SimpleClassWithExceptions())
                            .WithMethods(new List<MethodDescriptor>
                            {
                                MethodDescriptor.Create()
                                .WithId(2)
                                .WithParameterCount(0)
                                .WithParameters(new List<MethodParameterDescriptor>
                                {
                                    
                                })
                                .WithExecute((o, a) => method.Invoke(o, a))
                                .Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            var result = await methodExecutor.Execute(new MethodExecution<object>
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { }
            });

            Assert.Equal("Error", result.Error);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task AsyncMethodCalledAndAwaited()
        {
            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()
                            {
                                MethodDescriptor.Create()
                                .WithId(2)
                                .WithParameterCount(1)
                                .WithParameters(new List<MethodParameterDescriptor>
                                {
                                    new MethodParameterDescriptor(typeof(string), false)
                                })
                                .WithExecute((o, a) => Task.FromResult(a[0] as string))
                                .Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            const string Value = "expected";
            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { Value }
            });

            Assert.Equal(Value, result.Result as string);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task AsyncMethodExceptionPassedAlong()
        {
            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create().WithMethods(new List<MethodDescriptor>()
                            {
                                MethodDescriptor.Create()
                                .WithId(2)
                                .WithParameterCount(1)
                                .WithParameters(new List<MethodParameterDescriptor>
                                {
                                    new MethodParameterDescriptor(typeof(string), false)
                                })
                                .WithExecute((o, a) => Task.FromException(new NotSupportedException("Error")))
                                .Get()
                            }).WithId(1).Get() }
                        }), new NoopObjectParameterBinder());

            const string Value = "expected";
            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { Value }
            });

            Assert.Equal("Error", result.Error);
            Assert.False(result.Success);
        }
    }
}
