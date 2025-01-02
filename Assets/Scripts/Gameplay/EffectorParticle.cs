using Mirror;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorParticle : Effector
    {
        [SerializeField] private bool particlesPrefabs;
        [SerializeField] private ParticleSystem[] particles;
        [Space]
        [SerializeField] private bool audioSourcesPrefabs;
        [SerializeField] private AudioSource[] audioSources;
        [Space]
        [SerializeField] private bool overrideSpawnPoint;
        [ShowIf(nameof(overrideSpawnPoint))]
        [SerializeField] private Transform spawnPoint;

        Transform cachedTransform;


        private void Awake()
        {
            cachedTransform = transform;
        }

        [ClientRpc]
        protected override void PlayEffect()
        {
            if (particles != null)
            {
                foreach (var particle in particles)
                {
                    if (particlesPrefabs)
                    {
                        Instantiate(particle, SpawnPoint.position, SpawnPoint.rotation).Play();
                        continue;
                    }
                    particle.Play();
                }
            }

            if (audioSources != null)
            {
                foreach (var source in audioSources)
                {
                    if (audioSourcesPrefabs)
                    {
                        Instantiate(source, SpawnPoint.position, SpawnPoint.rotation).Play();
                        continue;
                    }
                    source.Play();
                }
            }
        }


        protected Transform SpawnPoint => overrideSpawnPoint ? spawnPoint : cachedTransform;
    }
}
