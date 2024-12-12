using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using System.Collections.Generic;
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

        [SyncVar(hook = nameof(ApplyFractionColor))]
        protected Color color;

        protected HashSet<Material> cachedMaterials;


        public override void Initialize(Entity sender)
        {
            lifetimeCancellationToken?.Dispose();
            lifetimeCancellationToken = new CancellationTokenSource();

            if (sender.TryFindAbillity<AbillityDataFraction>(out var abillityFraction) && abillityFraction.Has())
            {
                fraction = abillityFraction.Get();
                color = fraction.GetColor(fractionColor);
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

        protected virtual void ApplyFractionColor(Color oldColor, Color newColor)
        {
            Renderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            cachedMaterials ??= new(renderers.Length);

            foreach (Renderer renderer in renderers)
            {
                Material material = renderer.material;
                material.color = newColor;

                // Unity makes a clone of the Material every time Renderer.material is used.
                // We will Destroy it in OnDestroy to prevent a memory leak.
                cachedMaterials.Add(material);
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
            if (health.GetAbillityFraction().Get() == null)
                return true;

            // If the fraction is not our ally, we can hurt it
            if (fraction.GetFractionStatus(health.GetAbillityFraction().Get()) != FractionStatus.Ally)
                return true;

            return false;
        }


        private void OnDestroy()
        {
            if (cachedMaterials == null)
                return;

            foreach(Material material in cachedMaterials)
            {
                // Destroy it to prevent a memory leak.
                Destroy(material);
            }
        }
    }
}
