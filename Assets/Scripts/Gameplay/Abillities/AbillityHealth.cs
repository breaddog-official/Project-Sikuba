using Mirror;
using UnityEngine;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealth : Abillity
    {
        [SerializeField, Min(0f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float initialHealth = 100f;

        [field: SyncVar]
        public float Health { get; protected set; }

        AbillityFraction cachedAbillityFraction;


        public override void Initialize()
        {
            base.Initialize();
            Health = initialHealth;
        }

        [Server]
        public void Hurt(float damage)
        {
            print($"hurt for {damage}");
            float modifiedDamage = ModifyDamage(damage);

            Health -= modifiedDamage;
        }

        [Server]
        public void Heal(float amount)
        {
            print($"heal for {amount}");
            float modifiedAmount = ModifyHeal(amount);

            Health += modifiedAmount;
        }




        protected virtual float ModifyDamage(float damage)
        {
            return Mathf.Clamp(damage, 0f, Health);
        }

        protected virtual float ModifyHeal(float amount)
        {
            return Mathf.Clamp(amount, 0f, maxHealth - Health);
        }



        /// <summary>
        /// Optimization for once GetComponent (AbillityHealth) call (for example in bullet script)
        /// </summary>
        public AbillityFraction GetAbillityFraction()
        {
            if (cachedAbillityFraction == null && TryGetComponent<Entity>(out var entity))
                cachedAbillityFraction = entity.FindAbillity<AbillityFraction>();

            return cachedAbillityFraction;
        }
    }
}
