using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using UnityEngine;

namespace Scripts.UI
{
    public class SpawnerUI : MonoBehaviour
    {
        [SerializeField] private SessionManagerCommandMatch sessionManager;


        public void Spawn()
        {
            CommandMatchConfig config = new();

            sessionManager.SendRequestToSpawn(config);
        }
    }
}