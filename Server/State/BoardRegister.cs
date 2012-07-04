using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyncLightsServer.Server.State
{

    public struct Board
    {
        public IPEndPoint IEP;
        public DateTime LastContact;
        public bool Ready;
    }

    public class BoardRegister 
    {
        private static List<Board> boardList = new List<Board>();

        public static void Register(string host, int port)
        {
            boardList.Add(new Board() { IEP = new IPEndPoint(IPAddress.Parse(host), port), LastContact = DateTime.Now, Ready = true });
        }

        public static void Unregister(string host, int port)
        {
            lock (boardList)
            {
                IEnumerable<Board> query = boardList.Where(b => b.IEP.Address.ToString() == host && b.IEP.Port == port);
                if (query.ToArray().Length > 0)
                {
                    boardList.Remove(query.ToArray()[0]);
                }
            }
        }

        public static void SetReady(string host, int port)
        {
            lock (boardList)
            {
                Board board = boardList.Single(b => b.IEP.Address.ToString() == host && b.IEP.Port == port);
                boardList.Remove(board);
                board.Ready = true;
                boardList.Add(board);
                if (allReady())
                {
                    // Notify 
                    QueueProcessor.Notify();
                }
            }
        }

        public static void SetNotReady(string host, int port)
        {
            lock (boardList)
            {
                Board board = boardList.Single(b => b.IEP.Address.ToString() == host && b.IEP.Port == port);
                boardList.Remove(board);
                board.Ready = false;
                boardList.Add(board);
            }
        }

        public static bool allReady()
        {
            lock (boardList)
            {
                IEnumerable<Board> query = boardList.Where(b => b.Ready == true);
                List<Board> list = new List<Board>(query);
                return (list.Count == boardList.Count);
            }
        }

        public static List<Board> GetBoardList()
        {
            return boardList;
        }
    }
}
