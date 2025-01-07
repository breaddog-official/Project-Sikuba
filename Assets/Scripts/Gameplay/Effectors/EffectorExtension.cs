using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorExtension : Effector
    {
        [SerializeField] protected bool particlesPrefabs;
        [SerializeField] protected ParticleSystem[] particles;
        [Space]
        [SerializeField] protected bool audioSourcesPrefabs;
        [SerializeField] protected AudioSource[] audioSources;
        [Space]
        [SerializeField] protected bool overrideSpawnPoint;
        [ShowIf(nameof(overrideSpawnPoint))]
        [SerializeField] protected Transform spawnPoint;

        Transform cachedTransform;



        protected virtual void Awake()
        {
            cachedTransform = transform;
        }

        protected override void PlayEffect()
        {
            if (particles != null)
            {
                foreach (var particle in particles)
                {
                    if (particlesPrefabs)
                    {
                        PlayParticle(Instantiate(particle, SpawnPoint.position, SpawnPoint.rotation));
                    }
                    else
                        PlayParticle(particle);
                }
            }

            if (audioSources != null)
            {
                foreach (var source in audioSources)
                {
                    if (audioSourcesPrefabs)
                    {
                        PlaySource(Instantiate(source, SpawnPoint.position, SpawnPoint.rotation));
                    }
                    else
                        PlaySource(source);
                }
            }
        }


        protected virtual void PlayParticle(ParticleSystem particle)
        {
            particle.Play();
        }

        protected virtual void PlaySource(AudioSource source)
        {
            source.Play();
        }


        protected virtual Transform SpawnPoint => overrideSpawnPoint ? spawnPoint : cachedTransform;
    }
}
