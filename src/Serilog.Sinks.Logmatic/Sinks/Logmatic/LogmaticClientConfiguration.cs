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
        public byte MaxBackoff { get; set; } = 30;


        public LogmaticClientConfiguration(string ip = null, int port = 0, bool useSSL = true)
        {
            // setting-up defaults
            Ip = (ip == null) ? DEFAULT_LOGMATIC_IP : ip;
            UseSSL = useSSL;
            Port = port;
            if (Port == 0)
            {
                Port = (useSSL == true) ? DEFAULT_LOGMATIC_SSL_PORT : DEFAULT_LOGMATIC_PORT;
            }
        }
    }
}