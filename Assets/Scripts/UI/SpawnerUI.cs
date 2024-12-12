using Scripts.SessionManagers;
using UnityEngine;

namespace Scripts.UI
{
    public class SpawnerUI : MonoBehaviour
    {
        [SerializeField] private SessionManager sessionManager;


        public void Spawn()
        {
            sessionManager.SendRequestToSpawn();
        }
    }
}