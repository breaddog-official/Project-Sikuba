using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.SessionManagers;
using UnityEngine;

namespace Scripts.Gameplay.Fractions
{
    public class FractionSpawnItemGiver : MonoBehaviour
    {
        [SerializeField] protected SessionManagerDeathmatch sessionManager;
        [SerializeField] protected FractionDeathmatch fraction;
        [SerializeField] protected Item item;

        private void OnEnable()
        {
            fraction.OnSpawnEntity += GiveItem;
        }

        private void OnDisable()
        {
            fraction.OnSpawnEntity -= GiveItem;
        }


        public void GiveItem(Entity entity)
        {
            if (entity.TryFindAbillity<AbillityItemSocket>(out var socket))
            {
                Item spawnedItem = Instantiate(item, entity.transform.position, entity.transform.rotation);
                NetworkServer.Spawn(spawnedItem.gameObject);

                socket.EquipItem(spawnedItem);

                sessionManager.AddToSpawned(spawnedItem.gameObject);
            }
        }
    }
}
