using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace TCC.Sniffer
{
    public class SocketServer : TcpListener
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private List<TcpClient> _clients = new List<TcpClient>();

        public SocketServer(string ip = "127.0.0.1", int port = 9999) : base(IPAddress.Parse(ip), port)
        {
            Start();
            ListenForClients();
        }

        private void ListenForClients()
        {
            new Thread(() =>
            {
                while (Server.IsBound)
                {
                    TcpClient client = AcceptTcpClient();
                    _clients.Add(client);
                    logger.Debug("A client has connected.");
                }
            })
            .Start();
        }

        public void SendData(object data)
        {
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(data);

            // Add a newline to delimit different JSON objects on the other end.
            Array.Resize(ref json, json.Length + 1);
            json.SetValue(Encoding.UTF8.GetBytes("\n")[0], json.Length - 1);

            List<TcpClient> toRemove = new List<TcpClient>();

            _clients.ForEach(c =>
            {
                try
                { c.GetStream().Write(json); }
                catch
                // An error most likely means the client is no longer connected.
                // This could have a more explicit condition.
                { toRemove.Add(c); }
            });

            toRemove.ForEach(c =>
            {
                _clients.Remove(c);
                logger.Debug("A client has disconnected.");
            });
        }
    }
}
