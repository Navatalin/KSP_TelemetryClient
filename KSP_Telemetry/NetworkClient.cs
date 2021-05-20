using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace KSP_Telemetry
{
    public class NetworkClient
    {
        private System.Threading.Thread ClientT;
        private Socket server;
        private bool sendComplete = true;

        public void Connect()
        {
            server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect("localhost", 65000);
                ClientT.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Send(string data)
        {
            var dataToSend = Encoding.UTF8.GetBytes(data);
            if(server == null)
            {
                sendComplete = true;
                return;
            }

            if (sendComplete)
            {
                SendMessageToServerAsync(dataToSend);
            }
        }
        public void SendMessageToServerAsync(byte[] data)
        {
            
            SocketAsyncEventArgs socketAsync = new SocketAsyncEventArgs();
            socketAsync.SetBuffer(data, 0, data.Length);
            socketAsync.Completed += SendCompleted;
            server.SendAsync(socketAsync);
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            sendComplete = true;
        }


    }
}
