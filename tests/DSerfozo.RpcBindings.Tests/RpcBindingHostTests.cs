using System;
using System.Linq;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests
{
    public class RpcBindingHostTests
    {
        private class TestHost : RpcBindingHost<object>
        {
            public TestHost(IConnection<object> connection, Func<ICallbackFactory<object>, IParameterBinder<object>> parameterBinderFactory) : base(connection, parameterBinderFactory)
            {
            }
        }

        [Fact]
        public void RepositoryDisposed()
        {
            var connection = Mock.Of<IConnection<object>>();

            IBindingRepository bindingRepository;
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                bindingRepository = host.Repository;
            }

            Assert.Throws<ObjectDisposedException>(() => bindingRepository.AddBinding("sdf", new object()));
        }

        [Fact]
        public void NonExistingDynamicObjectRegistered()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                host.ResolvingBoundObject += (args) =>
                {
                    args.Object = new object();
                    return Task.CompletedTask; 
                };

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });

                Assert.Equal("testObj", host.Repository.Objects.Values.First().Name);
            }
        }

        [Fact]
        public void NonExistingDynamicObjectReturned()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                host.ResolvingBoundObject += (args) =>
                {
                    args.Object = new object();
                    return Task.CompletedTask;
                };

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    r.DynamicObjectResult.ObjectDescriptor.Name == "testObj" &&
                    r.DynamicObjectResult.Success)));
            }
        }


        [Fact]
        public void NonExistingDynamicObjectAddedAsDisposable()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var disposable = Mock.Of<IDisposable>();
            var disposableMock = Mock.Get(disposable);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                host.ResolvingBoundObject += (args) =>
                {
                    
                    args.Object = disposable;
                    args.Disposable = true;
                    return Task.CompletedTask;
                };

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
            }

            disposableMock.Verify(_ => _.Dispose());
        }

        [Fact]
        public void NonExistingDynamicObjectAddedAsNonDisposable()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var disposable = Mock.Of<IDisposable>();
            var disposableMock = Mock.Get(disposable);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                host.ResolvingBoundObject += (args) =>
                {

                    args.Object = disposable;
                    args.Disposable = false;
                    return Task.CompletedTask;
                };

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
            }

            disposableMock.Verify(_ => _.Dispose(), Times.Never);
        }

        [Fact]
        public void NonExistingDynamicObjectErrorReturned()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                host.ResolvingBoundObject += (name) => Task.FromException<object>(new Exception("Error"));

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    !r.DynamicObjectResult.Success &&
                    r.DynamicObjectResult.Exception == "Error")));
            }
        }

        [Fact]
        public void NonExistingDynamicObjectResolveErrorReturned()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                ResolvingBoundObjectArgs raisedArgs = null;
                host.ResolvingBoundObject += (name) => Task.CompletedTask;

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    !r.DynamicObjectResult.Success)));
            }
        }

        [Fact]
        public void ExistingDynamicObjectReturned()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder()))
            {
                var desc = host.Repository.AddBinding("testObj", new object());

                connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    r.DynamicObjectResult.Success &&
                    r.DynamicObjectResult.ObjectDescriptor == desc)));
            }
        }
    }
}
