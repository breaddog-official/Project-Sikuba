using Mirror;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityItemSocketFloating : AbillityItemSocket
    {
        [SerializeField] protected Rigidbody floatingArms;
        [SerializeField] protected float dropForce;


        [Command]
        public override void EquipItem(Item item)
        {
            base.EquipItem(item);

            // Realization
            if (item.TryGetComponent(out Joint joint))
            {
                joint.connectedBody = floatingArms;

                if (joint is FixedJoint && item.TryGetComponent(out Rigidbody rb))
                {
                    rb.MovePosition(floatingArms.position);
                    rb.MoveRotation(floatingArms.rotation);
                }
            }
        }

        [Command]
        public override void DropItem()
        {
            // Realization
            if (EquippedItem.TryGetComponent(out Joint joint))
            {
                joint.connectedBody = null;
            }

            if (EquippedItem.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(floatingArms.transform.forward * dropForce, ForceMode.Impulse);
            }

            base.DropItem();
        }
    }
}
