using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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

            //[Tooltip("Instantiate object on all clients (server only)")]
            //LocalRpc,

            [Tooltip("Instantiate object on local machine and spawn it on all clients (server only)")]
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
                for (int i = 0; i < objects.Length; i++)
                {
                    SpawnAndExecute(i);
                }
            }
        }


        protected abstract void ExecuteEffect(T obj);


        protected virtual void SpawnAndExecute(int effectIndex)
        {
            var effect = objects[effectIndex];
            T spawnedEffect = null;

            switch (effect.spawnMode)
            {
                case SpawnMode.AlreadyInScene:

                    spawnedEffect = effect.effect;
                    break;


                case SpawnMode.Local:

                    spawnedEffect = Instantiate(effect.effect, SpawnPoint.position, SpawnPoint.rotation);
                    break;


                /*case SpawnMode.LocalRpc:

                    SpawnAndExecuteRpc(effectIndex);
                    return;*/


                case SpawnMode.Server:

                    if (NetworkServer.active)
                    {
                        spawnedEffect = Instantiate(effect.effect, SpawnPoint.position, SpawnPoint.rotation);
                        NetworkServer.Spawn(spawnedEffect.gameObject);

                        break;
                    }

                    else
                    {
                        return;
                    }
            }

            if (spawnedEffect != null)
            {
                ExecuteEffect(spawnedEffect);
            }
        }

        /*[ClientRpc]
        protected virtual void SpawnAndExecuteRpc(int effectIndex)
        {
            var effect = objects[effectIndex];
            var spawnedEffect = Instantiate(effect.effect, SpawnPoint.position, SpawnPoint.rotation);

            ExecuteEffect(spawnedEffect);
        }*/

        protected virtual Transform SpawnPoint => overrideSpawnPoint ? spawnPoint : cachedTransform;


        [Serializable]
        protected struct Effect
        {
            public T effect;
            public SpawnMode spawnMode;
        }
    }
}
