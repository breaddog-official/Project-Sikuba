using UnityEngine;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;
using Scripts.SessionManagers;
using Mirror;
using NaughtyAttributes;
using Scripts.Gameplay.Abillities;

namespace Scripts.Gameplay.Fractions
{
    public class FractionDeathmatch : Fraction
    {
        [Header("Deathmatch")]
        [SerializeField] private SessionManagerDeathmatch sessionManager;
        [SerializeField] private Transform[] spawnPoints;
        [Space]
        [SerializeField] private bool dontRespawn;
        [HideIf(nameof(dontRespawn))]
        [SerializeField] private bool stunOnRespawn;
        [ShowIf(nameof(stunOnRespawn)), Min(0f)]
        [SerializeField] private float stunCooldown;

        [field: SyncVar]
        public uint Deaths { get; protected set; }

        private uint currentSpawnPoint;


        [ServerCallback]
        protected virtual void Start()
        {
            sessionManager.AddFraction(this);
        }

        [Server]
        public virtual void StartMatch()
        {
            Deaths = 0;

            foreach (var member in members)
            {
                member.gameObject.Teleportate(GetSpawnPoint());
            }
        }

        [Server]
        public virtual void StopMatch()
        {
            foreach (var member in members)
            {
                member.gameObject.Teleportate(sessionManager.GetSpawnPoint());
            }
        }

        public override bool Join(Entity entity)
        {
            if (base.Join(entity) == false)
                return false;


            Transform spawnPoint = GetSpawnPoint();

            entity.gameObject.Teleportate(spawnPoint);


            return true;
        }

        public override bool Leave(Entity entity)
        {
            if (base.Leave(entity) == false)
                return false;


            Transform spawnPoint = sessionManager.GetSpawnPoint();

            entity.gameObject.Teleportate(spawnPoint);


            return true;
        }



        public Transform GetSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return this.transform;

            Transform spawnPoint = spawnPoints[currentSpawnPoint];
            currentSpawnPoint.IncreaseInBounds(spawnPoints);

            return spawnPoint;
        }


        [Server]
        public virtual void HandleDeath(Entity entity)
        {
            if (entity.TryFindAbillity<AbillityItemSocket>(out var socket))
                socket.DropItem();

            Teleportate(entity);

            if (stunOnRespawn && !dontRespawn)
                entity.Stun(stunCooldown).Forget();

            Deaths++;

            if (dontRespawn && members.Count == Deaths)
            {
                sessionManager.SetLose(this);
            }
        }

        protected virtual void Teleportate(Entity entity)
        {
            // If dontRespawn is true, teleportates to lobby
            entity.gameObject.Teleportate(dontRespawn ? sessionManager.GetSpawnPoint() : GetSpawnPoint());
        }
    }
}
