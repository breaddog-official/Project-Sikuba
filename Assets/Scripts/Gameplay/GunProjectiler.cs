using Scripts.Extensions;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectiler : Item
    {
        [field: SerializeField] public bool IsAutomaticFire { get; protected set; }
        [field: SerializeField] public bool IsReloadable { get; protected set; }
        [field: SerializeField] public uint MaxAmmo { get; protected set; }
        [Space]
        [SerializeField] protected Transform projectilePrefab;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected ParticleSystem shootParticle;

        public uint CurrentAmmo { get; protected set; }



        public override void StartUsing()
        {
            shootParticle.IfNotNull(shootParticle.Play);
        }

        public override void StopUsing()
        {
            
        }
    }
}
