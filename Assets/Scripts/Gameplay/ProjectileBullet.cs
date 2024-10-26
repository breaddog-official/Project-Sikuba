using Cysharp.Threading.Tasks;
using Mirror;
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



        public override void Initialize()
        {
            lifetimeCancellationToken?.Dispose();
            lifetimeCancellationToken = new CancellationTokenSource();

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
            if (curHits < maxHits && collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);

                curHits++;
                if (curHits >= maxHits)
                {
                    DestroyBullet();
                }
            }
        }

        protected virtual void DestroyBullet()
        {
            if (NetworkServer.active)
                NetworkServer.UnSpawn(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
