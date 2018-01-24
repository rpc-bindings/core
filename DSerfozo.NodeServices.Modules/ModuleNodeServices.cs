using DSerfozo.NodeServices.Modules.Configuration;
using DSerfozo.NodeServices.Modules.RpcHost;
using DSerfozo.RpcBindings;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.NodeServices.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DSerfozo.NodeServices.Modules
{
    public class ModuleNodeServices : INodeServices
    {
        private readonly IDictionary<string, StringAsTempFile> bindingModules;
        private readonly ModuleNodeServicesOptions options;
        private readonly RpcBindingHost bindingHost;
        private readonly StringAsTempFile initScript;
        private readonly OutOfProcessNodeConnection connection;
        private readonly INodeServices nodeServices;
            
        public ModuleNodeServices(ModuleNodeServicesOptions options)
        {
            options.UseSocketHosting();
            nodeServices = NodeServicesFactory.CreateNodeServices(options);
            initScript = new StringAsTempFile(EmbeddedResourceReader.Read(typeof(ModuleNodeServices), "/Content/Node/init.js"), options.ApplicationStoppingToken);
            connection = new OutOfProcessNodeConnection();
            bindingHost = new RpcBindingHost(connection);

            this.options = options;

            bindingModules = options.BuiltInModules
                .ToDictionary(k => k.Key, v => new StringAsTempFile(v.Value, options.ApplicationStoppingToken));

            options.BoundObjects.ToList().ForEach(kv => bindingHost.Repository.AddBinding(kv.Key, kv.Value));
        }

        public async Task Initialize()
        {
            var stream = await nodeServices.InvokeExportAsync<Stream>(initScript.FileName, 
                "initializeBinding", 
                bindingModules.ToDictionary(k => k.Key, v => v.Value.FileName),
                bindingHost.Repository.Objects);

            connection.Initialize(stream);
        }

        public Task<T> InvokeAsync<T>(string moduleName, params object[] args)
        {
            return nodeServices.InvokeAsync<T>(moduleName, args);
        }

        public Task<T> InvokeAsync<T>(CancellationToken cancellationToken, string moduleName, params object[] args)
        {
            return nodeServices.InvokeAsync<T>(cancellationToken, moduleName, args);
        }

        public Task<T> InvokeExportAsync<T>(string moduleName, string exportedFunctionName, params object[] args)
        {
            return nodeServices.InvokeExportAsync<T>(moduleName, exportedFunctionName, args);
        }

        public Task<T> InvokeExportAsync<T>(CancellationToken cancellationToken, string moduleName, string exportedFunctionName, params object[] args)
        {
            return nodeServices.InvokeExportAsync<T>(cancellationToken, moduleName, exportedFunctionName, args);
        }

        public void Dispose()
        {

        }
    }
}
