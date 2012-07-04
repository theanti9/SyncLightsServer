using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using SyncLightsServer.Server.State;

namespace SyncLightsServer.Server
{
    public class RequestHandler
    {
        public static byte[] Handle(string request)
        {

            Hashtable urlParams = parseUrlParams(request);
            // Need to have an action
            if (!urlParams.ContainsKey("action"))
            {
                string returnbody = "Missing 'action' parameter";
                string header = makeHeader(returnbody.Length);
                return Encoding.ASCII.GetBytes(header + returnbody);
            }
            // Make sure we have the right parameters
            if (!urlParams.ContainsKey("ip"))
            {
                string returnbody = "Missing 'ip' parameter";
                string header = makeHeader(returnbody.Length);
                return Encoding.ASCII.GetBytes(header + returnbody);
            }
            if (!urlParams.ContainsKey("port"))
            {
                string returnbody = "Missing 'port' parameter";
                string header = makeHeader(returnbody.Length);
                return Encoding.ASCII.GetBytes(header + returnbody);
            }

            string action = urlParams["action"].ToString();
            
            if (action == "register")
            {
                BoardRegister.Register(urlParams["ip"].ToString(), int.Parse(urlParams["port"].ToString()));
            }
            else if (action == "unregister") // if they're not registering, they're unregistring
            {
                BoardRegister.Unregister(urlParams["ip"].ToString(), int.Parse(urlParams["port"].ToString()));
            }
            else if (action == "queue")
            {
                PatternQueue.Enqueue(Pattern.Pattern.Parse(urlParams["pattern"].ToString()));
            }
            else if (action == "ready")
            {
                BoardRegister.SetReady(urlParams["ip"].ToString(), int.Parse(urlParams["port"].ToString()));
            }
            string returntext = "complete";
            string head = makeHeader(returntext.Length);
            return Encoding.ASCII.GetBytes(head + returntext);
        }

        private static Hashtable parseUrlParams(string request)
        {
            Hashtable parameters = new Hashtable();
            int q = request.IndexOf('?');
            if (q < 1)
            {
                return parameters;
            }
            string[] split = request.Substring(q + 1, request.IndexOf(' ', q) - q).Split(new char[] { '&' });
            foreach (string s in split)
            {
                string[] para = s.Split(new char[] { '=' });
                parameters.Add(HttpUtility.UrlDecode(para[0]), HttpUtility.UrlDecode(para[1]));
            }
            return parameters;
        }

        private static string makeHeader(int length)
        {
            return "HTTP/1.1 200\r\nContent-Type: text/html\r\nAccept-Ranges: bytes\r\nContent-Length: " + length.ToString() + "\r\n\r\n";

        }
    }
}
