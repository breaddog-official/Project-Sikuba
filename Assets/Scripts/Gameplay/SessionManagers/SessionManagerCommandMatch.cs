using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.SessionManagers
{
    public class SessionManagerCommandMatch : SessionManager
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;

        private uint currentSpawnPoint;


        protected override GameObject SpawnPlayerBeforeStart()
        {
            Transform spawnPoint = GetSpawnPoint();
            return Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        protected override void ConfigurePlayerBeforeStart(GameObject player)
        {
            if (player.TryGetComponent<Entity>(out var entity))
            {
                entity.Initialize();
            }
        }



        protected virtual Transform GetSpawnPoint()
        {
            Transform spawnPoint = spawnPoints[currentSpawnPoint];
            currentSpawnPoint.IncreaseInBounds(spawnPoints);

            return spawnPoint;
        }
    }
}
