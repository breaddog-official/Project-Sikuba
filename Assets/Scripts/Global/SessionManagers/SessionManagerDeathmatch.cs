using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.Extensions;
using Mirror;
using Scripts.Gameplay.Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;

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

        public enum LastFractionBehaviour
        {
            Nothing,

            [Tooltip("When the last NotStated fraction remains, it automatically becomes the Winner.")]
            AutoWin,
            [Tooltip("When the last NotStated fraction remains, it automatically becomes the Loser.")]
            AutoLose
        }

        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [Header("Gameplay")]
        [SerializeField] private MatchEndMode matchEndMode;
        [SerializeField] private LastFractionBehaviour lastFractionBehaviour;

        [field: SyncVar]
        public bool IsMatch { get; protected set; }

        protected readonly SyncDictionary<uint, FractionState> fractions = new();

        public event Action OnStartMatch;
        public event Action OnStopMatch;

        protected HashSet<GameObject> spawnedInSession;


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
            //if (player.TryGetComponent<Entity>(out var entity))
            //{
            //    entity.Initialize();
            //}
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
            spawnedInSession ??= new(512);

            foreach (var identity in spawnedInSession)
            {
                if (identity == null)
                    continue;

                NetworkServer.Destroy(identity);
            }

            spawnedInSession.Clear();

            foreach (var fraction in fractions)
            {
                if (fraction.Key.TryFindByID(out FractionDeathmatch fractionDeathmatch))
                    fractionDeathmatch.StartMatch();
            }

            matchStartTime = NetworkTime.time;

            OnStartMatchRpc();



            IsMatch = true;
            print("Start match");
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
            print("Stop match");
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
        public virtual void AddToSpawned(GameObject gameObject)
        {
            spawnedInSession.Add(gameObject);
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
            // Cache not stated fractions
            var notStatedFractions = new HashSet<KeyValuePair<uint, FractionState>>();


            // First we process all stated fractions
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

                    case FractionState.NotStated:

                        // Cache not stated fraction
                        notStatedFractions.Add(fraction);
                        break;
                }
            }

            // After we process all not stated fractions
            if (lastFractionBehaviour != LastFractionBehaviour.Nothing)
            {
                if (notStatedFractions.Count() == 1)
                {
                    // Because we have auto behaviour, and this is last fraction not stated, marks him as choosed auto mode
                    fractions[notStatedFractions.First().Key] = lastFractionBehaviour switch
                    {
                        LastFractionBehaviour.AutoWin => FractionState.Winner,
                        LastFractionBehaviour.AutoLose => FractionState.Loser,
                        _ => FractionState.NotStated,
                    };

                    // Set because not NotStated
                    stated++;
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
