using Serilog.Debugging;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Serilog.Sinks.Logmatic
{
    public class LogmaticClient
    {
        private LogmaticClientConfiguration _clientConfig;
        private TcpClient _client;

        private SslStream _sslStream;
        private Stream _rawStream;
        // Simple wrapper depending of the connection type (ssl or not)
        private Stream _stream
        {
            get
            {
                return _clientConfig.UseSSL ? _sslStream : _rawStream;
            }
        }


        private static readonly UTF8Encoding UTF8 = new UTF8Encoding();

        
        public LogmaticClient(LogmaticClientConfiguration clientConfig)
        {
            _clientConfig = clientConfig;

        }

        // connect to the specify endpoint
        private void connect()
        {
            SelfLog.WriteLine("Starting a new connection to {0}:{1} (SSL = {2})", _clientConfig.Ip, _clientConfig.Port, _clientConfig.UseSSL);
            try
            {
                _client = new TcpClient();
                _client.ConnectAsync(_clientConfig.Ip, _clientConfig.Port).Wait();

                _rawStream = _client.GetStream();


                if (_clientConfig.UseSSL)
                {
                    _sslStream = new SslStream(_rawStream);
                    _sslStream.AuthenticateAsClientAsync(_clientConfig.Ip).Wait();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public void writeAndRetry(string payload)
        {

            for (int i = 0; i < _clientConfig.MaxRetries; i++)
            {

                // backoff mechanism
                int backoffTime = (int)Math.Min(Math.Pow(i, 2), _clientConfig.MaxBackoff);
                if (backoffTime > 0)
                {
                    SelfLog.WriteLine("Making a new attempt in {0} seconds ({1}/{2})", backoffTime, i, _clientConfig.MaxRetries);
                }
                Thread.Sleep(backoffTime * 1000);


                if (_client == null || _client.Connected == false)
                {
                    try
                    {
                        connect();
                    }
                    catch (Exception e)
                    {
                        SelfLog.WriteLine("Exception while connecting client: {0}", e);
                        continue;
                    }
                }

                try
                {
                    byte[] data = UTF8.GetBytes(payload);
                    _stream.Write(data, 0, data.Length);
                    return;
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Retry to send log event: {0}", e);
                }

            }
            SelfLog.WriteLine("Exception while sending log event, event dropped");

        }


        public void flush()
        {
            if (_stream != null)
            {
                try
                {
                    _stream.Flush();
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Exception while flushing client: {0}", e);
                }
            }
        }

        public void close()
        {
            if (_client != null)
            {
                try
                {
                    flush();

#if NETSTANDARD1_4
                    _client.Dispose();
#else
                    _client.Close();
#endif
                    _client = null;
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Exception while closing client: {0}", e);
                }
            }
        }



    }


}
