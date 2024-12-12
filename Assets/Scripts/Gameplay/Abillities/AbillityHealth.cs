using Mirror;
using UnityEngine;
using Scripts.Gameplay.Entities;
using Scripts.Extensions;
using System.Threading;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityHealth : Abillity
    {
        [Header("Health")]
        [SerializeField, Min(0f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float initialHealth = 100f;
        [Header("Dead")]
        [SerializeField] private float deadStunCooldown;
        [Header("Effectors")]
        [SerializeField] private Effector hurtEffector;
        [SerializeField] private Effector healEffector;
        [SerializeField] private Effector respawnEffector;
        [SerializeField] private Effector deadEffector;

        [field: SyncVar]
        public float Health { get; protected set; }

        AbillityDataFraction abillityFraction;
        PredictedRigidbody rb;

        CancellationTokenSource cancellationTokenSource;


        public override bool Initialize()
        {
            base.Initialize();

            rb = GetComponent<PredictedRigidbody>();
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
                Dead();

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

        [ServerCallback]
        protected virtual void OnDestroy()
        {
            cancellationTokenSource.ResetToken();
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
        protected virtual void Dead()
        {
            if (deadEffector != null)
                deadEffector.Play();

            Transform spawnPoint = abillityFraction.Get().GetSpawnPoint();

            rb.predictedRigidbody.MovePosition(spawnPoint.position);
            rb.predictedRigidbody.MoveRotation(spawnPoint.rotation);

            cancellationTokenSource.ResetToken();
            cancellationTokenSource = new();

            Entity.Stun(deadStunCooldown, cancellationTokenSource.Token).Forget();

            OnRespawn();
        }

        protected virtual void OnRespawn()
        {
            Health = initialHealth;

            if (respawnEffector != null)
                respawnEffector.Play();
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
    }
}
