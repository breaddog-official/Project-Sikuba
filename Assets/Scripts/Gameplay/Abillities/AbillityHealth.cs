using Mirror;
using UnityEngine;
using Scripts.Gameplay.Entities;
using Scripts.Extensions;
using System.Threading;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealth : Abillity
    {
        [Header("Health")]
        [SerializeField, Min(0f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float initialHealth = 100f;
        [Header("Effectors")]
        [SerializeField] private Effector hurtEffector;
        [SerializeField] private Effector healEffector;
        [SerializeField] private Effector respawnEffector;
        [SerializeField] private Effector deadEffector;

        [field: SyncVar]
        public float Health { get; protected set; }

        AbillityDataFraction abillityFraction;


        public override bool Initialize()
        {
            base.Initialize();

            abillityFraction = Entity.FindAbillity<AbillityDataFraction>();

            OnRespawn();

            return true;
        }

        [Server]
        public void Hurt(float damage)
        {
            print($"hurt for {damage}");
            float modifiedDamage = ModifyDamage(damage);

            Health -= modifiedDamage;

            if (Health <= 0)
                Death();

            else if (hurtEffector != null)
                hurtEffector.Play();
        }

        [Server]
        public void Heal(float amount)
        {
            print($"heal for {amount}");
            float modifiedAmount = ModifyHeal(amount);

            Health += modifiedAmount;

            if (healEffector != null)
                healEffector.Play();
        }



        protected virtual float ModifyDamage(float damage)
        {
            return Mathf.Clamp(damage, 0f, Health);
        }

        protected virtual float ModifyHeal(float amount)
        {
            return Mathf.Clamp(amount, 0f, maxHealth - Health);
        }



        [Server]
        protected virtual void Death()
        {
            if (deadEffector != null)
                deadEffector.Play();

            if (abillityFraction.Get() is FractionDeathmatch fraction)
                fraction.HandleDeath(Entity);

            OnRespawn();
        }

        protected virtual void OnRespawn()
        {
            if (respawnEffector != null)
                respawnEffector.Play();
        }

        public override void ResetState()
        {
            base.ResetState();

            Health = initialHealth;
        }



        /// <summary>
        /// Optimization for once GetComponent (AbillityHealth) call (for example in bullet script)
        /// </summary>
        public AbillityDataFraction GetAbillityFraction()
        {
            if (abillityFraction == null && Entity != null)
                abillityFraction = Entity.FindAbillity<AbillityDataFraction>();

            return abillityFraction;
        }


        public override void OnStopServer()
        {
            Death();
        }
    }
}
