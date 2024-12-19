using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;
using Mirror;
using Scripts.Gameplay.Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

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

        [field: SyncVar]
        public bool IsMatch { get; protected set; }

        protected readonly SyncDictionary<uint, FractionState> fractions = new();

        public event Action OnStartMatch;
        public event Action OnStopMatch;


        private uint currentSpawnPoint;
        [SyncVar] private double matchStartTime;
        [SyncVar] private double matchStopTime;




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



        [Server]
        public virtual void StartMatch()
        {
            fractions.SetAll(FractionState.NotStated);

            foreach (var fraction in fractions)
            {
                if (fraction.Key.TryFindByID(out FractionDeathmatch fractionDeathmatch))
                    fractionDeathmatch.StartMatch();
            }

            matchStartTime = NetworkTime.time;

            OnStartMatchRpc();



            IsMatch = true;
            print("SessionManager started match");
        }

        [Server]
        public virtual void StopMatch()
        {
            foreach (var fraction in fractions)
            {
                if (fraction.Key.TryFindByID(out FractionDeathmatch fractionDeathmatch))
                    fractionDeathmatch.StopMatch();
            }

            matchStopTime = NetworkTime.time;

            OnStopMatchRpc();



            IsMatch = false;
            print("SessionManager stoped match");
        }

        [ClientRpc] private void OnStartMatchRpc() => OnStartMatch?.Invoke();
        [ClientRpc] private void OnStopMatchRpc() => OnStopMatch?.Invoke();


        [Server]
        public virtual void AddFraction(FractionDeathmatch fraction)
        {
            if (fractions.Keys.Contains(fraction.netId) == false)
            {
                fractions.Add(fraction.netId, FractionState.NotStated);
            }
        }


        

        [Server]
        public virtual void SetWin(FractionDeathmatch fraction)
        {
            fractions[fraction.netId] = FractionState.Winner;
            print($"{fraction} is win");
            CheckMatchResults();
        }

        [Server]
        public virtual void SetLose(FractionDeathmatch fraction)
        {
            fractions[fraction.netId] = FractionState.Loser;
            print($"{fraction} is lose");
            CheckMatchResults();
        }


        [Server]
        public virtual void CheckMatchResults()
        {
            // Counter fractions states is not NotStated
            uint stated = 0;


            foreach (var fraction in fractions.ToArray())
            {
                switch (fraction.Value)
                {
                    case FractionState.Winner when matchEndMode == MatchEndMode.OneWinAllLose:

                        // Set all lose except current fraction
                        fractions.SetAll(FractionState.Loser, except: fraction.Key);

                        StopMatch();
                        return;

                    case FractionState.Winner or FractionState.Loser:

                        // Set because not NotStated
                        stated++;
                        break;

                    case FractionState.NotStated when stated == fractions.Count - 1 && autoWinLast:

                        // Because autoWinLast is true, and this is last fraction not stated, marks him a winner
                        fractions[fraction.Key] = FractionState.Winner;

                        // Set because not NotStated
                        stated++;
                        break;
                }
            }

            // If all stated, we can stop match
            if (stated == fractions.Count)
            {
                StopMatch();
            }
        }


        public IReadOnlyDictionary<uint, FractionState> GetFractions() => fractions;

        public double GetMatchTime() => matchStopTime - matchStartTime;
    }
}
