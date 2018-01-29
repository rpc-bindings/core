using DSerfozo.NodeServices.Modules.RpcHost;
//using DSerfozo.RpcBindings.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DSerfozo.NodeServices.Modules.Tests.RpcHost
{
    public class OutOfProcessNodeConnectionTests
    {
        //[Fact]
        //public async Task Sent()
        //{
        //    using (var memStream = new MemoryStream())
        //    {
        //        var connection = new OutOfProcessNodeConnection();
        //        connection.Initialize(memStream);

        //        await connection.Send(new RpcRequest<JToken>()
        //        {
        //            CallbackExecution = new CallbackExecution<JToken>()
        //            {
        //                Id = 1
        //            }
        //        });

        //        memStream.Seek(0, SeekOrigin.Begin);

        //        var written = new JsonSerializer().Deserialize(new JsonTextReader(new StreamReader(memStream)));

        //        Console.WriteLine(written);
        //    }
        //}
    }
}
