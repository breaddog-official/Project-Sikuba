using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using System.Threading;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class ProjectileBullet : Projectile
    {
        [SerializeField] protected float damage;
        [SerializeField] protected float lifetime;
        [SerializeField, Min(1)] protected uint maxHits = 1;

        protected uint curHits;
        protected float curLifetime;

        protected CancellationTokenSource lifetimeCancellationToken;

        protected Fraction fraction;



        public override void Initialize(Entity sender)
        {
            lifetimeCancellationToken?.Dispose();
            lifetimeCancellationToken = new CancellationTokenSource();

            if (sender.TryFindAbillity<AbillityFraction>(out var abillityFraction))
            {
                fraction = abillityFraction.GetFraction();
            }

            curHits = 0;
            curLifetime = 0;

            if (TryGetComponent<Rigidbody>(out var rb))
            { 
                rb.velocity = Vector3.zero;
            }

            Lifetime().Forget();
        }

        protected async UniTaskVoid Lifetime()
        {
            while (curLifetime < lifetime)
            {
                curLifetime += Time.deltaTime;

                await UniTask.NextFrame(lifetimeCancellationToken.Token);
            }

            DestroyBullet();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (curHits < maxHits)
            {
                if (collision.gameObject.TryGetComponent(out Entity entity) && entity.TryFindAbillity<AbillityHealth>(out var health))
                {
                    if (fraction.GetFractionStatus(health.GetAbillityFraction().GetFraction()) != FractionStatus.Ally)
                    {
                        health.Hurt(damage);
                    }
                }
                    

                curHits++;
                if (curHits >= maxHits)
                {
                    DestroyBullet();
                }
            }
        }

        protected virtual void DestroyBullet()
        {
            lifetimeCancellationToken.Cancel();

            if (NetworkServer.active)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
