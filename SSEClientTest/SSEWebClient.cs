using System;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace SSEClientTest
{
    class SSEWebClient
    {
        public void Start(string szUri)
        {
            int iNumMessage = 0;

            try
            {
                Uri LoginUri = new Uri(szUri);

                HttpWebRequest LoginRequest = (HttpWebRequest)WebRequest.Create(LoginUri);
                LoginRequest.Method = "GET";
                LoginRequest.Accept = "text/event-stream";
                LoginRequest.KeepAlive = true;
                LoginRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                LoginRequest.Headers.Add("Accept-Language", "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7");
                LoginRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                LoginRequest.Host = "live.meetscoresonline.com";
                LoginRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";

                LoginRequest.ReadWriteTimeout = 120000;
                LoginRequest.Timeout = 120000;

                try
                {
                    System.Diagnostics.Stopwatch timer = new Stopwatch();
                    timer.Start();

                    using (HttpWebResponse httpResponse = (HttpWebResponse)LoginRequest.GetResponse())
                    {
                        timer.Stop();
                        LogTransaction("PushConnexion", LoginRequest, httpResponse, timer.Elapsed);

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            try
                            {
                                using (Stream responseStream = httpResponse.GetResponseStream())
                                {
                                    var buffer = new byte[4096];
                                    int bytesRead;

                                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        iNumMessage++;

                                        SendEvent(System.Text.Encoding.Default.GetString(buffer, 0, bytesRead));

                                        if (responseStream.CanRead == false || iNumMessage > 10)
                                            break;
                                    }
                                }
                            }
                            catch (WebException wex2)
                            {
                                SendEvent("Stream WebException" + System.Environment.NewLine + wex2.Status.ToString() + System.Environment.NewLine + wex2.Message + System.Environment.NewLine + wex2.StackTrace);
                            }
                            catch (Exception ex2)
                            {
                                SendEvent("Stream Exception" + System.Environment.NewLine + ex2.Message);
                            }
                        }
                    }
                }
                catch (WebException wex1)
                {
                    SendEvent("HttpWebResponse WebException" + System.Environment.NewLine + wex1.Status.ToString() + System.Environment.NewLine + wex1.Message + System.Environment.NewLine + wex1.StackTrace);
                }
                catch (Exception ex1)
                {
                    SendEvent("HttpWebResponse Exception" + System.Environment.NewLine + ex1.Message);
                }
            }
            catch (WebException wex)
            {
                SendEvent("HttpWebRequest WebException" + System.Environment.NewLine + wex.Status.ToString() + System.Environment.NewLine + wex.Message + System.Environment.NewLine + wex.StackTrace);
            }
            catch (Exception ex)
            {
                SendEvent("HttpWebRequest Exception" + System.Environment.NewLine + ex.Message);
            }

            SendEvent("Start thread finish");
        }

        private void LogTransaction(string szName, HttpWebRequest LoginRequest, HttpWebResponse httpResponse, TimeSpan Elapsed)
        {
            Debug.WriteLine(System.Environment.NewLine + "Transaction : " + szName);
            Debug.WriteLine("Time : " + Elapsed.ToString() + System.Environment.NewLine);
            Debug.WriteLine("REQUEST");
            Debug.WriteLine("Uri : " + LoginRequest.RequestUri.ToString());
            Debug.WriteLine("Method : " + LoginRequest.Method);
            foreach (var header in LoginRequest.Headers)
                Debug.WriteLine(header.ToString() + ": " + LoginRequest.Headers.Get(header.ToString()));
            Debug.WriteLine(System.Environment.NewLine + "RESPONSE");
            Debug.WriteLine("Code : " + (int)httpResponse.StatusCode + " " + httpResponse.StatusCode.ToString());
            foreach (var header in httpResponse.Headers)
                Debug.WriteLine(header.ToString() + ": " + httpResponse.Headers.Get(header.ToString()));
            foreach (var cookie in httpResponse.Cookies)
                Debug.WriteLine(cookie.ToString());
            Debug.WriteLine(System.Environment.NewLine);
        }

        private void SendEvent(string szMessage)
        {
            SSEWebClientEventArgs args = new SSEWebClientEventArgs();
            args.szMessage = szMessage;
            OnSSEWebClientEvent(args);
        }

        public event EventHandler<SSEWebClientEventArgs> SSEWebClientEvent = null;

        protected virtual void OnSSEWebClientEvent(SSEWebClientEventArgs e)
        {
            if(SSEWebClientEvent != null)
                SSEWebClientEvent(this, e);
        }
    }

    public class SSEWebClientEventArgs : EventArgs
    {
        public string szMessage { get; set; }
    }
}
