using System;
using System.Threading;
using System.Diagnostics;

namespace SSEClientTest
{
    class Program
    {
        static void Main(string[] args)
        {

            SingleSSEClientThreadManager Client = new SingleSSEClientThreadManager("http://live.meetscoresonline.com/test-sse.aspx");
            Client.m_SSEWebClient.SSEWebClientEvent += OnSSEWebClientEvent;
            Client.Start();

            Thread.Sleep(5000);

            Client.Stop();

            Client.m_SSEWebClient.SSEWebClientEvent -= OnSSEWebClientEvent;
        }

        public static void OnSSEWebClientEvent(object sender, SSEWebClientEventArgs e)
        {
            Debug.WriteLine("Message : " + e.szMessage);
        }
    }
}
