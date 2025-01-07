using Mirror;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealthDeathmatch : AbillityHealth
    {
        [Header("Effectors")]
        [SerializeField] private Effector hurtEffector;
        [SerializeField] private Effector healEffector;
        [SerializeField] private Effector respawnEffector;
        [SerializeField] private Effector deadEffector;

        AbillityDataFraction abillityFraction;



        public override bool Initialize(Entity entity)
        {
            abillityFraction = entity.FindAbillity<AbillityDataFraction>();

            return base.Initialize(entity);
        }

        [Server]
        public override void Hurt(float damage)
        {
            base.Hurt(damage);

            if (hurtEffector != null)
                hurtEffector.PlayOnClients();
        }

        [Server]
        public override void Heal(float amount)
        {
            base.Heal(amount);

            if (healEffector != null)
                healEffector.PlayOnClients();
        }




        protected override void Death()
        {
            if (deadEffector != null)
                deadEffector.PlayOnClients();

            if (TryGetFraction(out var fraction))
                fraction.HandleDeath(Entity);

            base.Death();
        }

        protected override void Respawn()
        {
            if (respawnEffector != null)
                respawnEffector.PlayOnClients();

            base.Respawn();
        }




        private bool TryGetFraction(out FractionDeathmatch fraction)
        {
            fraction = null;
            if (abillityFraction != null && abillityFraction.Has() && abillityFraction.Get() is FractionDeathmatch fractionDeathmatch)
                fraction = fractionDeathmatch;

            return fraction != null;
        }


        public override void OnStopServer()
        {
            Death();
        }
    }
}
