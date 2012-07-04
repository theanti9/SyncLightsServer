using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using SyncLightsServer.Pattern;

namespace SyncLightsServer.Server.State
{
    public class PatternQueue
    {
        private static ConcurrentQueue<Pattern.Pattern> patternQueue = new ConcurrentQueue<Pattern.Pattern>();

        public static int QueueLength
        {
            get
            {
                return patternQueue.Count;
            }
        }

        public static void Enqueue(Pattern.Pattern p)
        {
            patternQueue.Enqueue(p);
            if (BoardRegister.allReady())
            {
                QueueProcessor.Notify();
            }
        }

        public static Pattern.Pattern Dequeue()
        {
            Pattern.Pattern p;
            bool success = patternQueue.TryDequeue(out p);
            return (success) ? p : null;
        }

        
    }
}
