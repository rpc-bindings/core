using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSerfozo.RpcBindings.Json
{
    public class LineDelimitedJsonConnection : IConnection<JToken>
    {
        private const int StreamBufferSize = 16 * 1024;
        private static readonly Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);
        private readonly JsonSerializer jsonSerializer;
        private Stream inputStream;
        private Stream outputStream;

        public event Action<RpcResponse<JToken>> RpcResponse;

        public LineDelimitedJsonConnection(JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public void Initialize(Stream inputStream, Stream outputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = outputStream;

            ReadLoop();
        }

        public async Task Send(RpcRequest<JToken> rpcRequest)
        {
            using (var writer = new StreamWriter(outputStream, Utf8EncodingWithoutBom, StreamBufferSize, true))
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.CloseOutput = false;

                jsonSerializer.Serialize(jsonWriter, rpcRequest);
                await writer.WriteLineAsync(stringWriter.ToString()).ConfigureAwait(false);
            }
        }

        private async void ReadLoop()
        {
            var streamReader = new StreamReader(inputStream);

            while (inputStream.CanRead)
            {
                var line = await streamReader.ReadLineAsync().ConfigureAwait(false);
                if (line == null)
                {
                    break;
                }
                var response = jsonSerializer.Deserialize<RpcResponse<JToken>>(new JsonTextReader(new StringReader(line)));
                RpcResponse?.Invoke(response);
            }
        }
    }
}
