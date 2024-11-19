using Mirror;
using Scripts.Gameplay.Fractions;
using System;
using UnityEngine;

namespace Scripts.SessionManagers
{
    /// <summary>
    /// Class for managing a game session (player spawn, fractions)
    /// </summary>
    public abstract class SessionManager<Config> : NetworkManager where Config : struct, NetworkMessage
    {
        [SerializeField] private Fraction[] initialFractions;

        public event Action OnSendRequestToSpawn;
        public event Action OnRecieveRequestToSpawn;



        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<Config>(RecieveRequestToSpawn);


            foreach (Fraction fraction in initialFractions)
            {
                NetworkServer.Spawn(fraction.gameObject);
            }
        }

        /// <summary>
        /// Sends request to spawn player
        /// </summary>
        [Client]
        public virtual void SendRequestToSpawn(Config message)
        {
            // Return if player already spawned
            if (NetworkClient.connection.identity != null)
                return;



            OnSendRequestToSpawn?.Invoke();

            NetworkClient.Send(message);
        }

        /// <summary>
        /// Configures and spawns player via config message
        /// </summary>
        [Server]
        protected void RecieveRequestToSpawn(NetworkConnectionToClient conn, Config message)
        {
            // Return if player already spawned
            if (conn.identity != null)
                return;



            OnRecieveRequestToSpawn?.Invoke();
            // playerPrefab is the one assigned in the inspector in Network
            // Manager but you can use different prefabs per race for example
            GameObject player = SpawnPlayerBeforeStart(message);

            // Apply data from the message however appropriate for your game
            ConfigurePlayerBeforeStart(player, message);

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, player);
        }




        protected abstract GameObject SpawnPlayerBeforeStart(Config message);

        protected abstract void ConfigurePlayerBeforeStart(GameObject player, Config config);
    }
}
