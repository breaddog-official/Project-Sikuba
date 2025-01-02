using Mirror;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealth : Abillity
    {
        [Header("Health")]
        [SerializeField, Min(0f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float initialHealth = 100f;

        [field: SyncVar]
        public float Health { get; protected set; }




        public override bool Initialize()
        {
            base.Initialize();

            Respawn();

            return true;
        }



        [Server]
        public virtual void Hurt(float damage)
        {
            float modifiedDamage = ModifyDamage(damage);

            Health -= modifiedDamage;

            if (Health <= 0)
                Death();
        }

        [Server]
        public virtual void Heal(float amount)
        {
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

            Health = initialHealth;
        }
    }
}
