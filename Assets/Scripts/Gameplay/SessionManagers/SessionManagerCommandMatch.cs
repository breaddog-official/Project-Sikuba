using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.SessionManagers
{
    public class SessionManagerCommandMatch : SessionManager<CommandMatchConfig>
    {
        [SerializeField] private Transform[] spawnPoints;

        private uint currentSpawnPoint;


        protected override GameObject SpawnPlayerBeforeStart(CommandMatchConfig message)
        {
            Transform spawnPoint = GetSpawnPoint();
            return Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        protected override void ConfigurePlayerBeforeStart(GameObject player, CommandMatchConfig config)
        {
            if (player.TryGetComponent<Entity>(out var entity))
            {
                entity.Initialize();
                //entity.FindAbillity<AbillityFraction>().IfNotNull(abillity => abillity.SetFraction(config.fraction));
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
