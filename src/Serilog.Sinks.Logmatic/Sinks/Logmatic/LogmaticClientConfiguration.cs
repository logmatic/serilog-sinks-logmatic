namespace Serilog.Sinks.Logmatic
{
    public class LogmaticClientConfiguration
    {

        private const string DEFAULT_LOGMATIC_IP = "api.logmatic.io";
        private const int DEFAULT_LOGMATIC_PORT = 10514;
        private const int DEFAULT_LOGMATIC_SSL_PORT = 10515;


        public string Ip { get; }
        public int Port { get; }
        public bool UseSSL { get; }
        public int MaxRetries { get; set; } = 10;
        public byte MaxBackoff { get; internal set; } = 30;

        private readonly int _port;

        public LogmaticClientConfiguration(string ip = DEFAULT_LOGMATIC_IP, int port = 0, bool useSSL = true)
        {
          
            UseSSL = useSSL;
            if (port == 0 )
            {
                Port = (useSSL == true) ? DEFAULT_LOGMATIC_SSL_PORT: DEFAULT_LOGMATIC_PORT;
            }
        }
    }
}