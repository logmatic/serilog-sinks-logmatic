using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Debugging;
using System.IO;
using System.Text;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Logmatic
{
    public class LogmaticSink : ILogEventSink, IDisposable
    {

        private readonly string _token;
        private readonly JsonFormatter _formatter = new JsonFormatter();
        private LogmaticClient _client;
        private readonly LogmaticClientConfiguration _clientConfig;

        public LogmaticSink(string token, LogmaticClientConfiguration clientConfiguration)
        {

            _token = token;
            _client = new LogmaticClient(_clientConfig);
        }

        public void Emit(LogEvent logEvent)
        {

            if (_client == null)
            {
                // connect or reconnect to the endpoint
                _client = new LogmaticClient(_clientConfig);
            }

            // format the event
            var payload = new StringBuilder();
            payload.AppendFormat("%s ", _token);
            using (var sw = new StringWriter(payload)) _formatter.Format(logEvent, sw);

            // send the event
            _client.writeAndRetry(payload.ToString());

        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.flush();
                _client.close();
            }
        }
    }
}