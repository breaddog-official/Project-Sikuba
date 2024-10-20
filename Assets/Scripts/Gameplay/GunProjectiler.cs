using Scripts.Extensions;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectiler : Gun
    {
        [field: SerializeField] public bool IsAutomaticFire { get; protected set; }
        [field: SerializeField] public override bool IsReloadable { get; protected set; }
        [field: SerializeField] public override uint MaxAmmo { get; protected set; }
        [Space]
        [SerializeField] protected Transform projectilePrefab;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected ParticleSystem shootParticle;

        public override uint CurrentAmmo { get; protected set; }



        public override bool StartFire()
        {
            shootParticle.IfNotNull(shootParticle.Play);
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

            // Realization

            return base.Reload();
        }
    }
}
