using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Extensions;
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
        [SerializeField] protected float lifetime = 3;
        [Space]
        [SerializeField, Min(1)] protected uint maxHits = 1;
        [SerializeField] protected bool ignoreHitsWhenDamageable;
        [Space]
        [SerializeField] private FractionColor fractionColor;
        [SerializeField] private Effector ricochetEffector;
        [SerializeField] private Effector destroyEffector;

        protected uint curHits;

        protected CancellationTokenSource lifetimeCancellationToken;

        protected Fraction fraction;



        public override void Initialize(Entity sender)
        {
            lifetimeCancellationToken?.Dispose();
            lifetimeCancellationToken = new CancellationTokenSource();

            if (sender.TryFindAbillity<AbillityFraction>(out var abillityFraction) && abillityFraction.HasFraction())
            {
                fraction = abillityFraction.GetFraction();
                ApplyFractionColor();
            }

            curHits = 0;

            if (TryGetComponent<Rigidbody>(out var rb))
            { 
                rb.velocity = Vector3.zero;
            }

            Lifetime().Forget();
        }

        protected async UniTaskVoid Lifetime()
        {
            await UniTask.Delay(lifetime.ConvertSecondsToMiliseconds(), cancellationToken: lifetimeCancellationToken.Token);

            DestroyBullet();
        }

        [ClientRpc]
        protected virtual void ApplyFractionColor()
        {
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.color = fraction.GetColor(fractionColor);
            }
        }

        [ServerCallback]
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (curHits < maxHits)
            {
                if (CanHurt(collision.gameObject, out AbillityHealth health))
                {
                    health.Hurt(damage);

                    if (ignoreHitsWhenDamageable)
                    {
                        DestroyBullet();
                        return;
                    }
                }
                    

                curHits++;
                if (curHits >= maxHits)
                {
                    DestroyBullet();
                    return;
                }

                else if (ricochetEffector != null)
                {
                    ricochetEffector.Play();
                }
            }
        }

        protected virtual void DestroyBullet()
        {
            lifetimeCancellationToken.Cancel();

            if (destroyEffector != null)
                destroyEffector.Play();



            if (NetworkServer.active)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }


        protected virtual bool CanHurt(GameObject gameObject, out AbillityHealth health)
        {
            health = null;

            // We can't hurt non Entity GameObject
            if (!gameObject.TryGetComponent(out Entity entity))
                return false;

            // We can't hurt if Entity don't have health abillity
            if (!entity.TryFindAbillity<AbillityHealth>(out health))
                return false;

            // Even if the Entity has no fraction, we can hit it. Fraction is the ability to skip a hurt, not to confirm
            if (health.GetAbillityFraction().GetFraction() == null)
                return true;

            // If the fraction is not our ally, we can hurt it
            if (fraction.GetFractionStatus(health.GetAbillityFraction().GetFraction()) != FractionStatus.Ally)
                return true;

            return false;
        }
    }
}
