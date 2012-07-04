using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SyncLightsServer.Server;
using SyncLightsServer.Server.State;

namespace SyncLightsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread queueRunner = new Thread(new ThreadStart(QueueProcessor.Handle));
            queueRunner.Start();
            Listener listener = new Listener(80);
            listener.Run();
        }
    }
}
