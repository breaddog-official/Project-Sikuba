using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using Scripts.Gameplay.ColorHandlers;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] protected bool enableMaterialsCaching;
        [SerializeField] protected ColorHandler[] colorHandlers;
        [Space]
        [SerializeField] private FractionColor fractionColor;
        [SerializeField] private Effector ricochetEffector;
        [SerializeField] private Effector destroyEffector;

        protected uint curHits;

        protected CancellationTokenSource lifetimeCancellationToken;

        [field: SyncVar(hook = nameof(ApplyFractionColor))]
        public Fraction Fraction { get; protected set; }

        protected readonly static Dictionary<Color, Material> cachedMaterials = new();


        public override void Initialize(Entity sender)
        {
            lifetimeCancellationToken?.Dispose();
            lifetimeCancellationToken = new CancellationTokenSource();

            if (sender.TryFindAbillity<AbillityDataFraction>(out var abillityFraction) && abillityFraction.Has())
            {
                Fraction = abillityFraction.Get();
            }

            curHits = 0;

            if (TryGetComponent<Rigidbody>(out var rb))
            { 
                rb.linearVelocity = Vector3.zero;
            }

            Lifetime().Forget();
        }

        protected async UniTaskVoid Lifetime()
        {
            await UniTask.Delay(lifetime.ConvertSecondsToMiliseconds(), cancellationToken: lifetimeCancellationToken.Token);

            DestroyBullet();
        }

        protected virtual void ApplyFractionColor(Fraction oldFraction, Fraction newFraction)
        {
            Color color = newFraction.GetColor(fractionColor);
            Material cachedMat = cachedMaterials.GetValueOrDefault(color);


            foreach (ColorHandler handler in colorHandlers)
            {
                // If handler is renderer, we need to optimize him
                if (handler is ColorHandlerRenderer renderer)
                {
                    if (enableMaterialsCaching && (cachedMat != null || cachedMaterials.TryGetValue(color, out cachedMat)))
                    {
                        renderer.SetMaterial(cachedMat);
                    }
                    else
                    {
                        // Creating new material
                        Material material = renderer.GetMaterial();
                        material.color = color;

                        // Unity makes a clone of the Material every time Renderer.material is used.
                        // We will Destroy it in OnDestroy to prevent a memory leak.
                        if (enableMaterialsCaching)
                            cachedMaterials.Add(color, material);
                    }
                }
                else
                {
                    handler.SetColor(color);
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            // We simulate bullet behaviour on clients because we need to know - show ricochet or dead particle
            if (curHits < maxHits)
            {
                if (NetworkServer.active && CanHurt(collision.gameObject, out AbillityHealth health))
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

        [ServerCallback]
        protected virtual void DestroyBullet()
        {
            lifetimeCancellationToken.Cancel();  

            NetworkServer.Destroy(gameObject);
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
            if (!entity.TryFindAbillity<AbillityDataFraction>(out var fractionData) || fractionData.Has() == false)
                return true;

            // If the fraction is not our ally, we can hurt it
            if (fractionData.Get().GetFractionStatus(fractionData.Get()) != FractionStatus.Ally)
                return true;

            return false;
        }


        private static void OnLevelWasLoaded(int level)
        {
            if (cachedMaterials.Count == 0)
                return;

            foreach (Material material in cachedMaterials.Values)
            {
                // If material already destroyed, skip him
                if (material == null)
                    continue;

                // Destroy it to prevent a memory leak.
                Destroy(material);
            }

            cachedMaterials.Clear();
        }

        private void OnDestroy()
        {
            if (destroyEffector != null && NetworkClient.active)
            {
                destroyEffector.Play();
            }
        }
    }
}
