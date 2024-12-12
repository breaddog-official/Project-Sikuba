using UnityEngine;
using Scripts.Gameplay.Entities;
using Mirror;

namespace Scripts.Gameplay.Abillities
{
    /// <summary>
    /// Class for equiping items
    /// </summary>
    public class AbillityEquiper : Abillity
    {
        protected AbillityItemSocket socket;


        public override bool Initialize()
        {
            if (gameObject.TryGetComponent(out Entity entity))
            {
                socket = entity.FindAbillity<AbillityItemSocket>();
                return true;
            }

            return false;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Item item) && socket != null)
            {
                if (item.Owner != null)
                    return;

                socket.EquipItem(item);
            }
        }
    }
}