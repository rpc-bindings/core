using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;

namespace DSerfozo.RpcBindings.Tests.Fixtures
{
    public class SimpleClassIgnore
    {
        [BindingIgnore]
        public string IgnoredProp { get; set; }

        public string NotIgnoredProp { get; set; }

        [BindingIgnore]
        public void Ignored()
        {

        }

        public void NotIngored()
        {

        }
    }
}
