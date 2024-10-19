using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectile : Gun
    {
        [field: SerializeField] public override bool IsReloadable { get; protected set; }
        [field: SerializeField] public override uint MaxAmmo { get; protected set; }
        [Space]
        [SerializeField] protected Transform ProjectilePrefab;
        [SerializeField] protected Animator Animator;

        public override uint CurrentAmmo { get; protected set; }



        public override bool StartFire()
        {
            return false;
        }

        public override bool StopFire()
        {
            return false;
        }


        public override bool Reload()
        {
            if (!IsReloadable)
                return false;

            return base.Reload();
        }
    }
}
