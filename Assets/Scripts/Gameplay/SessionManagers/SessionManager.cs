using Mirror;
using System;
using UnityEngine;

namespace Scripts.SessionManagers
{
    /// <summary>
    /// Class for managing a game session (player spawn, fractions)
    /// </summary>
    public abstract class SessionManager : NetworkBehaviour
    {
        public event Action OnSendRequestToSpawn;
        public event Action OnRecieveRequestToSpawn;

        /// <summary>
        /// Like Start, but only when start or connect to server. 
        /// </summary>
        public event Action OnStartMultiplayer;
        public event Action OnSpawnPlayer;



        public override void OnStartServer()
        {
            NetworkServer.ReplaceHandler<AddPlayerMessage>(RecieveRequestToSpawn);

            if (!NetworkClient.active)
                OnStartMultiplayer?.Invoke();
        }

        public override void OnStartClient()
        {
            if (!NetworkServer.active)
                OnStartMultiplayer?.Invoke();
        }

        /// <summary>
        /// Sends request to spawn player
        /// </summary>
        [Client]
        public virtual void SendRequestToSpawn(AddPlayerMessage message = default)
        {
            // Return if player already spawned
            if (NetworkClient.connection.identity != null)
                return;



            OnSendRequestToSpawn?.Invoke();

            NetworkClient.Send(message);
        }

        /// <summary>
        /// Configures and spawns player
        /// </summary>
        [Server]
        protected void RecieveRequestToSpawn(NetworkConnectionToClient conn, AddPlayerMessage message)
        {
            // Return if player already spawned
            if (conn.identity != null)
                return;



            OnRecieveRequestToSpawn?.Invoke();

            GameObject player = SpawnPlayerBeforeStart();

            // Apply data from the message however appropriate for your game
            ConfigurePlayerBeforeStart(player);

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, player);


            // Notify player about its spawn
            NotifyAboutSpawn(conn);
        }


        [TargetRpc]
        private void NotifyAboutSpawn(NetworkConnectionToClient target)
        {
            OnSpawnPlayer?.Invoke();
        }



        protected abstract GameObject SpawnPlayerBeforeStart();

        protected abstract void ConfigurePlayerBeforeStart(GameObject player);
    }
}
