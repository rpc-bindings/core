using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Marshaling;
using Microsoft.Reactive.Testing;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests
{
    public class RpcBindingHostTests
    {
        private class TestHost : RpcBindingHost<object>
        {
            public TestHost(IConnection<object> connection,
                Func<ICallbackFactory<object>, IParameterBinder<object>> parameterBinderFactory,
                IScheduler baseScheduler) : base(connection, parameterBinderFactory,
                baseScheduler)
            {
            }
        }

        public class ConnectionStub : IConnection<object>
        {
            public readonly ISubject<RpcResponse<object>> IncomingSubject = new Subject<RpcResponse<object>>();
            public readonly ISubject<RpcRequest<object>, RpcRequest<object>> OutgoingSubject = new Subject<RpcRequest<object>>();

            public bool IsOpen => true;

            public IDisposable Subscribe(IObserver<RpcResponse<object>> observer)
            {
                return IncomingSubject.Subscribe(observer);
            }

            public virtual void Send(RpcRequest<object> rpcRequest)
            {
                OutgoingSubject.OnNext(rpcRequest);
            }
        }

        [Fact]
        public void RepositoryDisposed()
        {
            var connection = Mock.Of<IConnection<object>>();

            IBindingRepository bindingRepository;
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), new TestScheduler()))
            {
                bindingRepository = host.Repository;
            }

            Assert.Throws<ObjectDisposedException>(() => bindingRepository.AddBinding("sdf", new object()));
        }

        [Fact]
        public void SchedulerDisposed()
        {
            var connection = Mock.Of<IConnection<object>>();

            IBindingRepository bindingRepository;
            var baseSchedulerMock = new Mock<IScheduler>();
            var disp = baseSchedulerMock.As<IDisposable>();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseSchedulerMock.Object))
            {
                bindingRepository = host.Repository;
            }

            disp.Verify(_ => _.Dispose());
        }

        [Fact]
        public void NonExistingDynamicObjectRegistered()
        {
            var baseScheduler= new TestScheduler();
            var connection = new ConnectionStub();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                host.ResolvingBoundObject += (s, args) =>
                {
                    args.Object = new object();
                };

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);

                Assert.Equal("testObj", host.Repository.Objects.Values.First().Name);
            }
        }

        [Fact]
        public void NonExistingDynamicObjectReturned()
        {
            var connection = Mock.Of<ConnectionStub>();
            var connectionMock = Mock.Get(connection);

            var baseScheduler = new TestScheduler();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                host.ResolvingBoundObject += (s, args) =>
                {
                    args.Object = new object();
                };

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    r.DynamicObjectResult.ObjectDescriptor.Name == "testObj" &&
                    r.DynamicObjectResult.Success)));
            }
        }

        [Fact]
        public void NonExistingDynamicObjectAddedAsDisposable()
        {
            var disposable = Mock.Of<IDisposable>();
            var disposableMock = Mock.Get(disposable);

            var baseScheduler = new TestScheduler();
            var connection = new ConnectionStub();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                host.ResolvingBoundObject += (s, args) =>
                {

                    args.Object = disposable;
                    args.Disposable = true;
                };

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);
            }

            disposableMock.Verify(_ => _.Dispose());
        }

        [Fact]
        public void NonExistingDynamicObjectAddedAsNonDisposable()
        {
            var disposable = Mock.Of<IDisposable>();
            var disposableMock = Mock.Get(disposable);

            var baseScheduler = new TestScheduler();
            var connection = new ConnectionStub();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                host.ResolvingBoundObject += (s, args) =>
                {

                    args.Object = disposable;
                    args.Disposable = false;
                };

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest()
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);
            }

            disposableMock.Verify(_ => _.Dispose(), Times.Never);
        }

        [Fact]
        public void NonExistingDynamicObjectErrorReturned()
        {
            var connection = Mock.Of<ConnectionStub>();
            var connectionMock = Mock.Get(connection);

            var baseScheduler = new TestScheduler();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                host.ResolvingBoundObject += (s, args) => throw new Exception("Error");

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    !r.DynamicObjectResult.Success &&
                    r.DynamicObjectResult.Exception == "Error")));
            }
        }

        [Fact]
        public void NonExistingDynamicObjectResolveErrorReturned()
        {
            var connection = Mock.Of<ConnectionStub>();
            var connectionMock = Mock.Get(connection);

            var baseScheduler = new TestScheduler();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    !r.DynamicObjectResult.Success)));
            }
        }

        [Fact]
        public void ExistingDynamicObjectReturned()
        {
            var connection = Mock.Of<ConnectionStub>();
            var connectionMock = Mock.Get(connection);

            var baseScheduler = new TestScheduler();
            using (var host = new TestHost(connection, factory => new NoopObjectParameterBinder(), baseScheduler))
            {
                var desc = host.Repository.AddBinding("testObj", new object());

                baseScheduler.Start();
                connection.IncomingSubject.OnNext(new RpcResponse<object>
                {
                    DynamicObjectRequest = new DynamicObjectRequest
                    {
                        ExecutionId = 1,
                        Name = "testObj"
                    }
                });
                baseScheduler.AdvanceBy(1);

                connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(r =>
                    r.DynamicObjectResult.ExecutionId == 1 &&
                    r.DynamicObjectResult.Success &&
                    r.DynamicObjectResult.ObjectDescriptor == desc)));
            }
        }
    }
}
