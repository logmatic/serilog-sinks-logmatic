using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Logmatic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Logmatic() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationLogmaticExtensions
    {


        /// <summary>
        /// Adds a sink that writes log events to the Logmatic.io. 
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        public static LoggerConfiguration Logmatic(
            this LoggerSinkConfiguration loggerConfiguration,
            string logmaticAPIKey,
            LogmaticClientConfiguration logmaticClientConfiguration = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {

            // checking mandatory args
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (string.IsNullOrWhiteSpace(logmaticAPIKey)) throw new ArgumentNullException("APIKey");

            // setting logmatic configuration
            LogmaticClientConfiguration clientConf = (logmaticClientConfiguration == null) ? new LogmaticClientConfiguration() : logmaticClientConfiguration;

            return loggerConfiguration.Sink(new LogmaticSink(logmaticAPIKey, clientConf), restrictedToMinimumLevel);
        }

    }
}
