using Mirror;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityGunSocketFloating : AbillityGunSocket
    {
        [SerializeField] protected Rigidbody floatingArms;
        [SerializeField] protected float dropGunForce;


        [Command]
        public override void EquipGun(Gun gun)
        {
            base.EquipGun(gun);

            // Realization
            if (gun.TryGetComponent(out Joint joint))
            {
                joint.connectedBody = floatingArms;

                if (joint is FixedJoint && gun.TryGetComponent(out Rigidbody rb))
                {
                    rb.MovePosition(floatingArms.position);
                    rb.MoveRotation(floatingArms.rotation);
                }
            }
        }

        [Command]
        public override void DropGun()
        {
            // Realization
            if (EquippedGun.TryGetComponent(out Joint joint))
            {
                joint.connectedBody = null;
            }

            if (EquippedGun.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(floatingArms.transform.forward * dropGunForce, ForceMode.Impulse);
            }

            base.DropGun();
        }
    }
}
