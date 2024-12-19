using UnityEngine;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;
using Scripts.SessionManagers;
using Mirror;
using NaughtyAttributes;
using Scripts.Gameplay.Abillities;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Scripts.Gameplay.Fractions
{
    public class FractionDeathmatch : Fraction
    {
        [Header("Deathmatch")]
        [SerializeField] private SessionManagerDeathmatch sessionManager;
        [SerializeField] private Transform[] spawnPoints;
        [Header("Gameplay")]
        [SerializeField] protected bool loseIfAllDead;
        [Header("Respawn")]
        [SerializeField] private bool dontRespawn;
        [HideIf(nameof(dontRespawn))]
        [SerializeField] private bool stunOnRespawn;
        [ShowIf(nameof(stunOnRespawn)), Min(0f)]
        [SerializeField] private float stunCooldown;
        [Header("Spawn")]
        [SerializeField] protected AllowJoin allowJoin;

        public readonly SyncHashSet<Entity> Alives = new();


        private uint currentSpawnPoint;

        protected enum TeleportateWhere
        {
            Lobby,
            Spawn
        }

        protected enum AllowJoin
        {
            Always,
            BeforeMatchStarted,
            BeforeMatchStartedAndOneTime
        }


        [ServerCallback]
        protected virtual void Start()
        {
            sessionManager.AddFraction(this);
        }

        [Server]
        public virtual void StartMatch()
        {
            Alives.Clear();
            Alives.AddRange(members);

            foreach (var member in members)
            {
                Teleportate(member, TeleportateWhere.Spawn);
            }
            print($"{Name} fraction started match");
        }

        [Server]
        public virtual void StopMatch()
        {
            foreach (var member in members.ToArray())
            {
                if (member == null)
                    continue;

                member.ResetState();
                Teleportate(member, TeleportateWhere.Lobby);
            }
            print($"{Name} fraction stoped match");
        }

        public override bool Join(Entity entity)
        {
            // If can't join, return
            if (base.Join(entity) == false)
                return false;

            // Add to alive entities
            if (sessionManager.IsMatch)
                Alives.Add(entity);

            // Reset all abillities
            entity.ResetState();

            if (entity.TryFindAbillity<AbillityDataFraction>(out var fraction))
            {
                fraction.Set(this);
            }


            return true;
        }

        public override bool Leave(Entity entity)
        {
            // If can't leave, return
            if (base.Leave(entity) == false)
                return false;

            // Remove from alive entities
            if (sessionManager.IsMatch)
                Alives.Remove(entity);

            // Reset all abillities
            entity.ResetState();

            // Teleportate to lobby
            Teleportate(entity, TeleportateWhere.Lobby);


            RenewFractionState();


            return true;
        }


        protected override bool CanJoin(Entity entity)
        {
            return base.CanJoin(entity) && allowJoin switch
            {
                AllowJoin.Always => true,
                AllowJoin.BeforeMatchStarted when !sessionManager.IsMatch => true,
                AllowJoin.BeforeMatchStartedAndOneTime when !sessionManager.IsMatch
                            && entity.TryFindAbillity<AbillityDataFraction>(out var fraction) && !fraction.Has() => true,
                _ => false
            };
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
            // Reset state of entity and all it abillities
            entity.ResetState();

            // Remove from Alives if entity will not respawn
            if (dontRespawn)
                Alives.Remove(entity);

            Teleportate(entity, TeleportateWhere.Spawn);

            // Stun
            if (stunOnRespawn && !dontRespawn)
                entity.Stun(stunCooldown).Forget();

            RenewFractionState();
        }

        protected virtual void Teleportate(Entity entity, TeleportateWhere where)
        {
            // If dontRespawn is true, teleportates to lobby
            entity.gameObject.Teleportate(where switch
            {
                TeleportateWhere.Spawn when !sessionManager.IsMatch => GetSpawnPoint(),    // Spawn, when match is not begin
                TeleportateWhere.Spawn when dontRespawn => sessionManager.GetSpawnPoint(), // Lobby, when want to spawn, but 'dontSpawn'
                TeleportateWhere.Lobby or _ => sessionManager.GetSpawnPoint()              // Lobby
            });
        }

        protected virtual void RenewFractionState()
        {
            // If all fraction dead, fraction is lose
            if (sessionManager.IsMatch && loseIfAllDead && Alives.Count == 0)
            {
                sessionManager.SetLose(this);
            }
        }
    }
}
