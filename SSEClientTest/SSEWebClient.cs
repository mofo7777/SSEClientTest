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
                LoginRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                LoginRequest.KeepAlive = true;
                LoginRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                LoginRequest.Headers.Add("Accept-Language", "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7");
                LoginRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                LoginRequest.Host = "live.meetscoresonline.com";
                LoginRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";

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

                                        SSEWebClientEventArgs args = new SSEWebClientEventArgs();
                                        args.szMessage = System.Text.Encoding.Default.GetString(buffer, 0, bytesRead);
                                        OnSSEWebClientEvent(args);

                                        if (responseStream.CanRead == false || iNumMessage > 10)
                                            break;
                                    }
                                }
                            }
                            catch (WebException wex2)
                            {
                                Debug.WriteLine(wex2.Message, "WebException2");
                                Debug.WriteLine(wex2.StackTrace, "WebException2");
                                Debug.WriteLine(wex2.Status.ToString(), "WebException2");
                            }
                            catch (Exception ex2)
                            {
                                Debug.WriteLine(ex2.Message + System.Environment.NewLine + ex2.Source, "Exception2");
                            }
                        }
                    }
                }
                catch (WebException wex1)
                {
                    Debug.WriteLine(wex1.Message, "WebException1");
                    Debug.WriteLine(wex1.StackTrace, "WebException1");
                    Debug.WriteLine(wex1.Status.ToString(), "WebException1");
                }
                catch (Exception ex1)
                {
                    Debug.WriteLine(ex1.Message + System.Environment.NewLine + ex1.Source, "Exception1");
                }
            }
            catch (WebException wex)
            {
                Debug.WriteLine(wex.Message, "WebException");
                Debug.WriteLine(wex.StackTrace, "WebException");
                Debug.WriteLine(wex.Status.ToString(), "WebException");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + System.Environment.NewLine + ex.Source, "Exception");
            }

            Debug.WriteLine("Start thread finish");
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

        public event EventHandler<SSEWebClientEventArgs> SSEWebClientEvent;

        protected virtual void OnSSEWebClientEvent(SSEWebClientEventArgs e)
        {
            EventHandler<SSEWebClientEventArgs> handler = SSEWebClientEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class SSEWebClientEventArgs : EventArgs
    {
        public string szMessage { get; set; }
    }
}
