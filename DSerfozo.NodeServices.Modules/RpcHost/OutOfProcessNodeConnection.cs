using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Communication.Model;

namespace DSerfozo.NodeServices.Modules.RpcHost
{
    public class OutOfProcessNodeConnection : IConnection<JToken>
    {
        private const int StreamBufferSize = 16 * 1024;
        private static readonly Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);
        private Stream stream;

        public event Action<RpcResponse<JToken>> RpcResponse;

        public void Initialize(Stream stream)
        {
            this.stream = stream;

            ReadLoop();
        }

        public async Task Send(RpcRequest<JToken> rpcRequest)
        {
            var serialized = JsonConvert.SerializeObject(rpcRequest);
            using (var writer = new StreamWriter(stream, Utf8EncodingWithoutBom, StreamBufferSize, true))
            {
                await writer.WriteLineAsync(serialized).ConfigureAwait(false);
            }
        }

        private async void ReadLoop()
        {
            var serializer = new JsonSerializer();
            var streamReader = new StreamReader(stream);

            while(stream.CanRead)
            {
                var line = await streamReader.ReadLineAsync().ConfigureAwait(false);
                var execution = JsonConvert.DeserializeObject<RpcResponse<JToken>>(line);
                RpcResponse?.Invoke(execution);
            }
        }
    }
}
