using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SyncLightsServer.Pattern
{
    public class Pattern
    {
        private List<PatternNode> _list;
        private int _length;
        private byte _time;

        #region Public Properties

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public byte Time
        {
            get
            {
                return _time;
            }
        }

        #endregion

        public Pattern()
        {
            _list = new List<PatternNode>();
            _length = 0;
            _time = 0;
        }

        public void Add(PatternNode node)
        {
            _list.Add(node);
            _length++;
        }

        public void SetTime(byte t)
        {
            _time = t;
        }

        public byte[] ToBytes()
        {
            byte[] buff = new byte[6 + 8 * _length];
            buff[0] = 0x01;
            Array.Copy(BitConverter.GetBytes(h2n(_length)), 0,buff, 1, 4);
            buff[5] = _time;
            int j = 6;
            foreach (PatternNode node in _list)
            {
                Array.Copy(BitConverter.GetBytes(h2n(node.color)), 0, buff, j, 4);
                j += 4;
                buff[j] = node.x;
                j++;
                buff[j] = node.y;
                j++;
                Array.Copy(BitConverter.GetBytes(h2n(node.time_delay)), 0, buff, j, 2);
                j += 2;
            }
            return buff;
        }


        public static Pattern Parse(string pString)
        {
            Pattern p = new Pattern();
            string[] pSplit = pString.Split(new char[] { ',' });
            int len = int.Parse(pSplit[0]);
            p.SetTime((byte)int.Parse(pSplit[1]));
            int j = 2;

            for (int count = 0; count < len; count++)
            {
                int color = 0;

                color |= (byte)int.Parse(pSplit[j]);
                color <<= 8;
                j++;

                color |= (byte)int.Parse(pSplit[j]);
                color <<= 8;
                j++;

                color |= (byte)int.Parse(pSplit[j]);
                j++;

                byte x = (byte)int.Parse(pSplit[j]);
                j++;

                byte y = (byte)int.Parse(pSplit[j]);
                j++;

                short tdelay = short.Parse(pSplit[j]);
                j++;

                p.Add(new PatternNode() { color = color, time_delay = tdelay, x = x, y = y });
            }


            return p;
        }

        private int h2n(int i)
        {
            return IPAddress.HostToNetworkOrder(i);
        }
        private short h2n(short i)
        {
            return IPAddress.HostToNetworkOrder(i);
        }
    }
}
