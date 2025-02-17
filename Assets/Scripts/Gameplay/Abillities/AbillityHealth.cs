using Mirror;
using System;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealth : Abillity
    {
        [field: Header("Health")]
        [field: SerializeField, Min(0f)] public float MaxHealth { get; protected set; } = 100f;
        [field: SerializeField, Min(0f)] public float InitialHealth { get; protected set; } = 100f;

        [field: SyncVar(hook = nameof(UpdateHealthHook))]
        public float Health { get; protected set; }

        public event Action<float, float> OnHealthChanged;



        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            Respawn();

            return true;
        }



        [Server]
        protected virtual void SetHealth(float health)
        {
            //OnHealthChanged?.Invoke(Health, health);
            Health = health;
        }

        [Server]
        public virtual void Hurt(float damage)
        {
            float modifiedDamage = ModifyDamage(damage);

            //OnHealthChanged?.Invoke(Health, Health - modifiedDamage);

            Health -= modifiedDamage;


            if (Health <= 0)
                Death();
        }

        [Server]
        public virtual void Heal(float amount)
        {
            float modifiedAmount = ModifyHeal(amount);

            //OnHealthChanged?.Invoke(Health, Health + modifiedAmount);

            Health += modifiedAmount;
        }



        protected virtual float ModifyDamage(float damage)
        {
            return Mathf.Clamp(damage, 0f, Health);
        }

        protected virtual float ModifyHeal(float amount)
        {
            return Mathf.Clamp(amount, 0f, MaxHealth - Health);
        }



        protected virtual void UpdateHealthHook(float oldHealth, float newHealth)
        {
            OnHealthChanged?.Invoke(oldHealth, newHealth);
        }

        



        [Server]
        protected virtual void Death()
        {
            Respawn();
        }

        [Server]
        protected virtual void Respawn()
        {
            ResetState();
        }

        public override void ResetState()
        {
            base.ResetState();

            SetHealth(InitialHealth);
        }
    }
}
