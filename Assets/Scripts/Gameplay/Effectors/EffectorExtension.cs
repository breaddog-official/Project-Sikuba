using Mirror;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    /// <summary>
    /// Effector extension for spawnable or simple effects like particle or sound
    /// </summary>
    public abstract class EffectorExtension<T> : Effector where T : Component
    {
        public enum SpawnMode
        {
            [Tooltip("Dont spawn or instantiate object")]
            AlreadyInScene,

            [Tooltip("Instantiate object on local machine")]
            Local,

            [Tooltip("Instantiate object on local machine and spawn it on all observers (server only)")]
            Server
        }

        [SerializeField] protected Effect[] objects;
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
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    ExecuteEffect(SpawnOrGet(obj));
                }
            }
        }


        protected abstract void ExecuteEffect(T obj);


        protected virtual T SpawnOrGet(Effect effect)
        {
            var spawned = effect.spawnMode == SpawnMode.AlreadyInScene ? effect.effect : Instantiate(effect.effect, SpawnPoint.position, SpawnPoint.rotation);

            if (effect.spawnMode == SpawnMode.Server)
                NetworkServer.Spawn(spawned.gameObject);

            return spawned;
        }



        protected virtual Transform SpawnPoint => overrideSpawnPoint ? spawnPoint : cachedTransform;


        [Serializable]
        protected struct Effect
        {
            public T effect;
            public SpawnMode spawnMode;
        }
    }
}
