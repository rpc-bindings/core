using Microsoft.AspNetCore.NodeServices;
using System.Collections.Generic;

namespace DSerfozo.NodeServices.Modules.Configuration
{
    public class ModuleNodeServicesOptions : NodeServicesOptions
    {
        public IDictionary<string, string> BuiltInModules { get; set; }

        public IDictionary<string, object> BoundObjects { get; set; }
    }
}
