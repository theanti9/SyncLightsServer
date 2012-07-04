using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SyncLightsServer.Server.State
{
    public class QueueProcessor
    {

        private static AutoResetEvent go = new AutoResetEvent(false);

        public static void Notify()
        {
            go.Set();
        }

        public static void Handle()
        {
            // Wait for initial go signal
            go.WaitOne();
            while (true)
            {
                
                // Get the next Pattern out of the queue
                Pattern.Pattern pattern = PatternQueue.Dequeue();

                List<Board> _list = BoardRegister.GetBoardList();
                // Handle the list in a parallel loop
                if (pattern != null && _list.Count > 0)
                {
                    Parallel.ForEach(_list, curBoard =>
                    {
                        try
                        {
                            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            s.Connect(curBoard.IEP);
                            s.Send(pattern.ToBytes());
                            s.Receive(new byte[10]);
                            s.Shutdown(SocketShutdown.Both);
                            s.Close();

                            BoardRegister.SetNotReady(curBoard.IEP.Address.ToString(), curBoard.IEP.Port);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    });
                }
                // Wait until we're told to go again
                go.WaitOne();
            }
        }

    }
}
