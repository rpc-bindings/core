using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Tests.Fixtures
{
    public class SimpleClassWithMethods
    {
        public virtual void MethodWithoutReturn()
        {

        }

        public virtual void MethodWithParameters(int intParam, double doubleParam, string stringParam)
        {

        }

        [return:BindValue]
        public virtual string MethodWithResult(string input)
        {
            return input;
        }
    }
}
