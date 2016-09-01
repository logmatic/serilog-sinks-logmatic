using System;
using Serilog.Events;
using System.IO;
using System.Text;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.Logmatic
{
    public class LogmaticSink : PeriodicBatchingSink
    {

        private readonly string _token;
        private readonly JsonFormatter _formatter = new JsonFormatter();
        private LogmaticClient _client;
        private readonly LogmaticClientConfiguration _clientConfig;


        public LogmaticSink(string token, LogmaticClientConfiguration clientConfiguration, TimeSpan period, int batchSizeLimit) : base(batchSizeLimit, period)
        {
            _token = token;
            _clientConfig = clientConfiguration;
        }


        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {

            if (!events.Any()) return;

            lock (this)
            {
                if (_client == null)
                {
                    // connect or reconnect to the endpoint
                    _client = new LogmaticClient(_clientConfig);
                }
            }

            // format the event
            var payload = new StringBuilder();
            var sw = new StringWriter(payload);

            foreach (var logEvent in events)
            {
                payload.Append(_token).Append(" ");
                _formatter.Format(logEvent, sw);
            }
            // send the event
            _client.writeAndRetry(payload.ToString());

        }

        protected override void Dispose(bool disposing)
        {
            if (_client != null)
            {
                _client.flush();
                _client.close();
                _client = null;
            }
            base.Dispose(disposing);
        }
    }
}