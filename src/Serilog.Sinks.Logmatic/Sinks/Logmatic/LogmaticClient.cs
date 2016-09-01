using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace Serilog.Sinks.Logmatic
{
    public class LogmaticClient
    {
        private LogmaticClientConfiguration _clientConfig;
        private SslStream _sslStream;
        private Stream _rawStream;
        private TcpClient _client;

        static readonly UTF8Encoding UTF8 = new UTF8Encoding();

        public LogmaticClient(LogmaticClientConfiguration clientConfig)
        {
            _clientConfig = clientConfig;
            connect();


        }

        private void connect()
        {
            SelfLog.WriteLine("Starting a new connection to {0}:{1} (SSL = {2})", _clientConfig.Ip , _clientConfig.Port, _clientConfig.UseSSL);

            _client = new TcpClient();
            _client.ConnectAsync(_clientConfig.Ip, _clientConfig.Port).Wait();

            _rawStream = _client.GetStream();

            if (_clientConfig.UseSSL)
            {
                _sslStream = new SslStream(_rawStream);
                _sslStream.AuthenticateAsClientAsync(_clientConfig.Ip).Wait();
            }
        }

        // Simple wrapper depending of the connection type (ssl or not)
        private Stream _stream
        {
            get
            {
                return _clientConfig.UseSSL ? _sslStream : _rawStream;
            }
        }

        public void writeAndRetry(string payload)
        {

            for (int i = 0; i < _clientConfig.MaxRetries; i++)
            {

                // backoff mechanism
                int backoffTime = (int) Math.Min(Math.Pow(i, 2), _clientConfig.MaxBackoff) ;
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
                        SelfLog.WriteLine("Unable to connect client: {0}", e);
                        continue;
                    }
                }

                try
                {
                    byte[] data = UTF8.GetBytes(payload);
                    _stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Unable to write send log line: {0}", e);
                }

            }
            
        }

        
        public void flush()
        {
            if (_stream != null)
            {
                try
                {
                    _stream.Flush();
                } catch(Exception e)
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
                    _client.Dispose();
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Exception while closing client: {0}", e);
                }
            }
        }



    }


}
