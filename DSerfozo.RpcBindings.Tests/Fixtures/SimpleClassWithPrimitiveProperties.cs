namespace DSerfozo.RpcBindings.Tests.Fixtures
{
    public class SimpleClassWithPrimitiveProperties
    {
        public int IntProperty { get; set; }

        public double DoubleProperty { get; set; }

        public string StringProperty { get; private set; }

        public SimpleClassWithPrimitiveProperties(string str)
        {
            StringProperty = str;
        }
    }
}
