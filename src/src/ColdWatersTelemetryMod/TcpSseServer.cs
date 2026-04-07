using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ColdWatersMod
{
    internal class SimpleSseTcpServer
    {
        internal class ClientConnection
        {
            public TcpClient Client;
            public NetworkStream Stream;
        }

        private TcpListener listener;
        private bool running;

        private readonly List<ClientConnection> clients = new List<ClientConnection>();
        private readonly object syncRoot = new object();

        private readonly byte[] buffer = new byte[4096];

        public event EventHandler<EventArgs> OnConnected;

        public void Start(string host)
        {
            if (this.running) return;

            string[] hostChunks = host.Split(':');
            IPAddress endpoint = IPAddress.Parse(hostChunks[0]);
            int port = int.Parse(hostChunks[1]);

            this.listener = new TcpListener(endpoint, port);
            this.listener.Start();
            this.running = true;

            this.listener.BeginAcceptTcpClient(this.OnClientAccepted, null);
        }

        public void Stop()
        {
            this.running = false;

            lock (this.syncRoot)
            {
                foreach (ClientConnection c in this.clients)
                {
                    try { c.Stream.Close(); } catch { }
                    try { c.Client.Close(); } catch { }
                }
                this.clients.Clear();
            }

            try { this.listener.Stop(); } catch { }
        }

        public void WriteMessage(string message)
        {
            string sse = string.Concat("data: ", message, "\n\n");
            byte[] data = Encoding.UTF8.GetBytes(sse);

            List<ClientConnection> toRemove = null;

            lock (this.syncRoot)
            {
                foreach (ClientConnection c in this.clients)
                {
                    try
                    {
                        c.Stream.BeginWrite(data, 0, data.Length, this.OnWriteCompleted, c);
                    }
                    catch
                    {
                        if (toRemove == null) toRemove = new List<ClientConnection>();
                        toRemove.Add(c);
                    }
                }

                if (toRemove != null)
                {
                    foreach (ClientConnection c in toRemove)
                    {
                        this.clients.Remove(c);
                        try { c.Stream.Close(); } catch { }
                        try { c.Client.Close(); } catch { }
                    }
                }
            }
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            if (!this.running) return;

            TcpClient client = null;

            try
            {
                client = this.listener.EndAcceptTcpClient(ar);
            }
            catch
            {
                return;
            }

            NetworkStream stream = client.GetStream();

            stream.BeginRead(this.buffer, 0, this.buffer.Length, this.OnRequestRead, new ClientConnection
            {
                Client = client,
                Stream = stream
            });

            

            this.listener.BeginAcceptTcpClient(this.OnClientAccepted, null);
        }

        private void OnRequestRead(IAsyncResult ar)
        {
            ClientConnection conn = (ClientConnection)ar.AsyncState;

            int bytesRead = 0;
            try
            {
                bytesRead = conn.Stream.EndRead(ar);
            }
            catch
            {
                this.CloseClient(conn);
                return;
            }

            if (bytesRead <= 0)
            {
                this.CloseClient(conn);
                return;
            }

            string request = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);

            string headers = "HTTP/1.1 200 OK\r\nContent-Type: text/event-stream\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\n\r\n";

            byte[] headerBytes = Encoding.ASCII.GetBytes(headers);

            try
            {
                conn.Stream.BeginWrite(headerBytes, 0, headerBytes.Length, this.OnHeadersSent, conn);
            }
            catch
            {
                this.CloseClient(conn);
            }
        }

        private void OnHeadersSent(IAsyncResult ar)
        {
            ClientConnection conn = (ClientConnection)ar.AsyncState;

            try
            {
                conn.Stream.EndWrite(ar);
                this.OnConnectedInvoke(conn);
            }
            catch
            {
                this.CloseClient(conn);
                return;
            }

            lock (this.syncRoot)
            {
                this.clients.Add(conn);
            }
        }

        protected virtual void OnConnectedInvoke(ClientConnection conn)
        {
            try
            {
                this.OnConnected?.Invoke(this, EventArgs.Empty);
            }
            catch
            {

            }
        }

        private void OnWriteCompleted(IAsyncResult ar)
        {
            ClientConnection conn = (ClientConnection)ar.AsyncState;

            try
            {
                conn.Stream.EndWrite(ar);
            }
            catch
            {
                this.CloseClient(conn);
            }
        }

        private void CloseClient(ClientConnection conn)
        {
            lock (this.syncRoot)
            {
                this.clients.Remove(conn);
            }

            try { conn.Stream.Close(); } catch { }
            try { conn.Client.Close(); } catch { }
        }
    }
}