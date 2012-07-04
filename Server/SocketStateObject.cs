using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SyncLightsServer.Server
{
    public class SocketStateObject
    {
        public Socket workSocket = null;
        public const int BUFFER_SIZE = 2048;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();
    }
}
