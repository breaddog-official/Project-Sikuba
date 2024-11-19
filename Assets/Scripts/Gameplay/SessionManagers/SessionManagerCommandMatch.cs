using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.SessionManagers
{
    public class SessionManagerCommandMatch : SessionManager<CommandMatchConfig>
    {
        protected override GameObject SpawnPlayerBeforeStart(CommandMatchConfig message)
        {
            Transform spawnPoint = message.fraction.GetSpawnPoint();
            return Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        protected override void ConfigurePlayerBeforeStart(GameObject player, CommandMatchConfig config)
        {
            if (player.TryGetComponent<Entity>(out var entity))
            {
                entity.Initialize();
                entity.FindAbillity<AbillityFraction>().IfNotNull(abillity => abillity.SetFraction(config.fraction));
            }
        }
    }
}
