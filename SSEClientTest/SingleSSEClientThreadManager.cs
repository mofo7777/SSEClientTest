using System.Threading.Tasks;
using System.Diagnostics;

namespace SSEClientTest
{
    class SingleSSEClientThreadManager
    {
        public SSEWebClient m_SSEWebClient = null;
        private string m_szUri { get; set; }
        private Task m_Task = null;

        public SingleSSEClientThreadManager(string szUri)
        {
            m_szUri = szUri;
            m_SSEWebClient = new SSEWebClient();
        }

        public bool Start()
        {
            if (m_Task != null)
            {
                Debug.WriteLine("SingleSSEClientThreadManager Start : m_Task != null");
                return false;
            }

            m_Task = Task.Run(() => m_SSEWebClient.Start(m_szUri));

            Debug.WriteLine("SingleSSEClientThreadManager Start : " + m_Task.Status.ToString());
            Debug.WriteLine("SingleSSEClientThreadManager Start : OK");

            return true;
        }

        public bool Stop()
        {
            if (m_Task == null)
            {
                Debug.WriteLine("SingleSSEClientThreadManager Stop : m_Task == null");
                return false;
            }

            Debug.WriteLine("SingleSSEClientThreadManager Stop 1 : " + m_Task.Status.ToString());
            m_Task.Wait();
            Debug.WriteLine("SingleSSEClientThreadManager Stop 2 : " + m_Task.Status.ToString());

            m_Task.Dispose();
            m_Task = null;

            Debug.WriteLine("SingleSSEClientThreadManager Stop : OK");

            return true;
        }
    }
}
