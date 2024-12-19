using UnityEngine;
using Mirror;

namespace Scripts.Network
{
    public class AdvancedPlayersHide : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected GameObject playerPrefab;


        protected virtual void OnEnable()
        {
            NetworkClient.RegisterPrefab(playerPrefab, SpawnHandler, UnspawnHandler);
        }

        protected virtual void OnDisable()
        {
            NetworkClient.UnregisterPrefab(playerPrefab);
        }


        protected virtual GameObject SpawnHandler(SpawnMessage msg)
        {
            return null;    
        }

        protected virtual void UnspawnHandler(GameObject spawned)
        {

        }
    }
}