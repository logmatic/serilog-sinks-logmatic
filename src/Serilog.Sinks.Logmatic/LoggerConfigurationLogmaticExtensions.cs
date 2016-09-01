using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Logmatic;
using System;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Logmatic() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationLogmaticExtensions
    {


        public static TimeSpan period = TimeSpan.FromSeconds(2);
        public static int maxNumberOfEventsPerBatch = 100;

        /// <summary>
        /// Adds a sink that writes log events to the Logmatic.io. 
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration</param>
        /// <param name="logmaticAPIKey">The API KEY that is use to send events to Logmatic.io</param>
        /// <param name="ip">The Logmatic.io Ip</param>
        /// <param name="port">The Logmatic.io port</param>
        /// <param name="useSSL">Use an encrypted connection (default: true)</param>
        /// <param name="logmaticClientConfiguration">Override the default Logmatic client configuration. If set, ip, port and useSSL are omitted</param>
        /// <param name="restrictedToMinimumLevel">The minimum logger level</param>
        public static LoggerConfiguration Logmatic(
            this LoggerSinkConfiguration loggerConfiguration,
            string logmaticAPIKey,
            string ip = null,
            int port = 0,
            bool useSSL = true,
            LogmaticClientConfiguration logmaticClientConfiguration = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {

            // checking mandatory args
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (string.IsNullOrWhiteSpace(logmaticAPIKey)) throw new ArgumentNullException("APIKey");

            // setting logmatic configuration
            LogmaticClientConfiguration clientConf = (logmaticClientConfiguration == null) ? new LogmaticClientConfiguration(ip, port, useSSL) : logmaticClientConfiguration;

            return loggerConfiguration.Sink(
                new LogmaticSink(
                    logmaticAPIKey,
                    clientConf,
                    period,
                    maxNumberOfEventsPerBatch),
                restrictedToMinimumLevel);
        }

    }
}
