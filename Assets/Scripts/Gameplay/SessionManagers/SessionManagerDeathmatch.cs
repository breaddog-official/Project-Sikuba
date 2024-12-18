using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;
using Mirror;
using Scripts.Gameplay.Fractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.SessionManagers
{
    public class SessionManagerDeathmatch : SessionManager
    {
        public enum FractionState
        {
            Winner,
            NotStated,
            Loser
        }

        public enum MatchEndMode
        {
            [Tooltip("When one wins, nothing happens. Waiting for everyone to get the state")]
            OneWinAndWait,

            [Tooltip("When one wins, everyone automatically loses.")]
            OneWinAllLose
        }

        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [Header("Gameplay")]
        [SerializeField] private MatchEndMode matchEndMode;
        [SerializeField] private bool autoWinLast;

        protected readonly SyncHashSet<FractionDeathmatch> fractions = new();
        protected readonly SyncDictionary<FractionDeathmatch, FractionState> fractionStates = new();

        private uint currentSpawnPoint;




        protected override GameObject SpawnPlayerBeforeStart()
        {
            Transform spawnPoint = GetSpawnPoint();
            return Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        protected override void ConfigurePlayerBeforeStart(GameObject player)
        {
            if (player.TryGetComponent<Entity>(out var entity))
            {
                entity.Initialize();
            }
        }




        [Server]
        public virtual Transform GetSpawnPoint()
        {
            Transform spawnPoint = spawnPoints[currentSpawnPoint];
            currentSpawnPoint.IncreaseInBounds(spawnPoints);

            return spawnPoint;
        }




        public virtual void StartMatch()
        {
            SetAll(FractionState.NotStated);

            foreach (var fraction in fractions)
            {
                fraction.StartMatch();
            }

            print("SessionManager started match");
        }

        public virtual void StopMatch()
        {
            foreach (var fraction in fractions)
            {
                fraction.StopMatch();
            }

            print("SessionManager stoped match");
        }


        [Server]
        public virtual void AddFraction(FractionDeathmatch fraction)
        {
            if (fractions.Contains(fraction) == false)
            {
                fractions.Add(fraction);
                fractionStates.Add(fraction, FractionState.NotStated);
            }
        }


        

        [Server]
        public virtual void SetWin(FractionDeathmatch fraction)
        {
            fractionStates[fraction] = FractionState.Winner;

            CheckMatchResults();
        }

        [Server]
        public virtual void SetLose(FractionDeathmatch fraction)
        {
            fractionStates[fraction] = FractionState.Loser;

            CheckMatchResults();
        }

        [Server]
        public virtual void SetAll(FractionState state, params Fraction[] except)
        {
            foreach (var fraction in fractionStates)
            {
                if (except.Contains(fraction.Key))
                    continue;

                fractionStates[fraction.Key] = state;
            }
        }


        [Server]
        public virtual void CheckMatchResults()
        {
            // Counter fractions states is not NotStated
            uint stated = 0;

            foreach (var fractionState in fractionStates)
            {
                switch (fractionState.Value)
                {
                    case FractionState.Winner when matchEndMode == MatchEndMode.OneWinAllLose:

                        // Set all lose except current fractionState
                        SetAll(FractionState.Loser, except: fractionState.Key);

                        StopMatch();
                        return;

                    case FractionState.Winner or FractionState.Loser:

                        // Set because not NotStated
                        stated++;
                        break;

                    case FractionState.NotStated when stated == fractionStates.Count - 1 && autoWinLast:

                        // Because autoWinLast is true, and this is last fraction not stated, marks him a winner
                        fractionStates[fractionState.Key] = FractionState.Winner;

                        // Set because not NotStated
                        stated++;
                        break;
                }
            }

            // If all stated, we can stop match
            if (stated == fractionStates.Count)
            {
                StopMatch();
            }
        }


        public IReadOnlyCollection<FractionDeathmatch> GetFractions() => fractions;
        public IReadOnlyDictionary<FractionDeathmatch, FractionState> GetFractionStates() => fractionStates;
    }
}
