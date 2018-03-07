using System;
using System.Collections.Generic;
using System.Text;

namespace DSerfozo.RpcBindings.Tests.Fixtures
{
    public class SimpleClassWithExceptions
    {
        public void ThrowException()
        {
            throw new NotSupportedException("Error");
        }
    }
}
