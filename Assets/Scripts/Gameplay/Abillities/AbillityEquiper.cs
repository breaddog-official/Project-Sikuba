using UnityEngine;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Abillities
{
    /// <summary>
    /// Class for equiping items
    /// </summary>
    public class AbillityEquiper : Abillity
    {
        protected AbillityItemSocket socket;


        public override void Initialize()
        {
            if (gameObject.TryGetComponent(out Entity entity))
            {
                socket = entity.FindAbillity<AbillityItemSocket>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Item item) && socket != null)
            {
                socket.EquipItem(item);
            }
        }
    }
}