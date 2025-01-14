using Mirror;
using UnityEngine;

namespace Scripts.UI
{
    public class Menu : MonoBehaviour
    {
        #region Network

        protected NetworkManager NetworkManager => NetworkManager.singleton;

        public void StartClient() => NetworkManager.StartClient();
        public void StartHost() => NetworkManager.StartHost();
        public void StartServer() => NetworkManager.StartServer();

        public void SetAddress(string address) => NetworkManager.networkAddress = address;
        public void SetPort(string port)
        {
            if (NetworkManager.transport is PortTransport portTransport && ushort.TryParse(port, out ushort parsedPort))
            {
                portTransport.Port = parsedPort; 
            }
        }
        public void SetMaxConnections(string maxConnections)
        {
            if (int.TryParse(maxConnections, out int parsedMaxConnections) && parsedMaxConnections >= 0)
            {
                NetworkManager.maxConnections = parsedMaxConnections;
            }
        }

        #endregion



        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
