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

        public virtual string MethodWithResult(string input)
        {
            return input;
        }
    }
}
