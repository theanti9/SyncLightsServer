using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SyncLightsServer.Server
{
    public class Listener
    {
        private int port;
        private Socket server;
        static AutoResetEvent allDone = new AutoResetEvent(false);

        public Listener(int p)
        {
            port = p;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Run()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
            server.Bind(iep);

            Console.WriteLine("Server initialized");
            server.Listen(50);
            Console.WriteLine("Listening...");

            while (true)
            {
                server.BeginAccept(new AsyncCallback(AcceptCon), server);
                allDone.WaitOne();
            }
        }

        private void AcceptCon(IAsyncResult iar)
        {

            allDone.Set();
            Socket s = (Socket)iar.AsyncState;
            Socket s2 = s.EndAccept(iar);
            SocketStateObject state = new SocketStateObject();
            state.workSocket = s2;
            s2.BeginReceive(state.buffer, 0, SocketStateObject.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(Read), state);
        }

        private void Read(IAsyncResult iar)
        {
            try
            {
                SocketStateObject state = (SocketStateObject)iar.AsyncState;
                Socket s = state.workSocket;

                int read = s.EndReceive(iar);

                if (read > 0)
                {
                    string _in = Encoding.ASCII.GetString(state.buffer, 0, read);
                    Console.WriteLine(_in);
                    state.sb.Append(_in);
                }

                if (state.sb.ToString().Length > 48)   // 48 is the bare minimum length for an incoming request
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(state.sb.ToString());
                    Console.WriteLine();
                    Console.WriteLine();
                    byte[] answer = RequestHandler.Handle(state.sb.ToString());
                    state.workSocket.BeginSend(answer, 0, answer.Length, SocketFlags.None, new AsyncCallback(Send), state);
                }
                else
                {
                    state.workSocket.BeginReceive(state.buffer, 0, SocketStateObject.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(Read), state);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
        }

        private void Send(IAsyncResult iar)
        {
            try
            {
                SocketStateObject state = (SocketStateObject)iar.AsyncState;
                state.workSocket.EndSend(iar);
                state.workSocket.Shutdown(SocketShutdown.Both);
                state.workSocket.Close();
            } 
            catch (Exception) 
            {
                // Do nothing
            }
        }

    }
}
