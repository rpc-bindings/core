using DSerfozo.RpcBindings.Calling;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using DSerfozo.RpcBindings.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
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

            Assert.IsType<ParameterMismatchException>(result.Error);
            Assert.False(result.IsSuccess);
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

            Assert.IsType<ParameterMismatchException>(result.Error);
            Assert.False(result.IsSuccess);
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
            const string Key = "key";
            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                Key = Key,
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { Value }
            });

            Assert.Equal(Value, result.Result as string);
            Assert.Equal(Key, result.Key);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task MethodCallExceptionPassedAlong()
        {
            var method = typeof(SimpleClassWithExceptions).GetMethod("ThrowException");

            var methodExecutor = new MethodExecutor<object>(
                    new ReadOnlyDictionary<int, ObjectDescriptor>(
                        new Dictionary<int, ObjectDescriptor>()
                        {
                            { 1, ObjectDescriptor.Create()
                            .WithObject(new SimpleClassWithExceptions())
                            .WithMethods(new List<MethodDescriptor>()
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

            const string Value = "expected";
            var result = await methodExecutor.Execute(new MethodExecution<object>()
            {
                ObjectId = 1,
                MethodId = 2,
                Parameters = new object[] { }
            });

            Assert.IsType<NotSupportedException>(result.Error);
            Assert.False(result.IsSuccess);
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
            Assert.True(result.IsSuccess);
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
                                .WithExecute((o, a) => Task.FromException(new NotSupportedException()))
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

            Assert.IsType<NotSupportedException>(result.Error);
            Assert.False(result.IsSuccess);
        }
    }
}
