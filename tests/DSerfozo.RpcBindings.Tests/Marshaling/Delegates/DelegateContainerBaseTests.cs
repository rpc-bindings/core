using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling.Delegates;
using Moq;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling.Delegates
{
    public class DelegateContainerBaseTests
    {
        private class Shit
        {
            ~Shit()
            {
                
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Sun(ICallbackExecutor<object> callbackExecutor)
        {
            new DelegateContainerBase<object>(1, callbackExecutor, Mock.Of<IParameterBinder<object>>());
        }
        
        [Fact]
        public void CallbackDeleted()
        {
            var callbackExecutor = Mock.Of<ICallbackExecutor<object>>();
            var callbackExecutorMock = Mock.Get(callbackExecutor);
            callbackExecutorMock.SetupGet(_ => _.CanExecute).Returns(true);
            Sun(callbackExecutor);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            callbackExecutorMock.Verify(c => c.DeleteCallback(1));
        }
    }
}
