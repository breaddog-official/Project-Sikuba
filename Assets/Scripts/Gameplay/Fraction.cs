using Mirror;
using UnityEngine;
using System.Collections.Generic;
using Scripts.Extensions;
using System;

namespace Scripts.Gameplay.Fractions
{
    public class Fraction : NetworkBehaviour
    {
        [field: SerializeField] public string Name { get; private set; }
        [Space]
        [SerializeField] private FractionStatus defaultFractionStatus = FractionStatus.Neutral;
        [SerializeField] private FractionStatus fractionStatusForMembers = FractionStatus.Ally;
        [SerializeField] private Transform[] spawnPoints;

        private Dictionary<Fraction, FractionStatus> statusDictionary;


        private uint currentSpawnPoint;



        [ServerCallback]
        private void Awake()
        {
            statusDictionary = new(4);
        }


        [Server]
        public FractionStatus GetFractionStatus(Fraction fraction)
        {
            if (fraction == null)
                throw new ArgumentNullException(nameof(fraction));

            if (fraction == this)
                return fractionStatusForMembers;


            if (statusDictionary.TryGetValue(fraction, out FractionStatus status))
            {
                return status;
            }
            else
            {
                statusDictionary.Add(fraction, defaultFractionStatus);
                return defaultFractionStatus;
            }
        }
        [Server]
        public void SetFractionStatus(Fraction fraction, FractionStatus status)
        {
            if (fraction == null)
                return;

            if (fraction == this)
                return;


            if (statusDictionary.ContainsKey(fraction))
                statusDictionary[fraction] = status;
            else
                statusDictionary.Add(fraction, status);
        }



        public Transform GetSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return this.transform;

            Transform spawnPoint = spawnPoints[currentSpawnPoint];
            currentSpawnPoint.IncreaseInBounds(spawnPoints);

            return spawnPoint;
        }
    }
}
