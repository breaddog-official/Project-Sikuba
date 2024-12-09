using Mirror;
using UnityEngine;
using System.Collections.Generic;
using Scripts.Extensions;
using System;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Abillities;

namespace Scripts.Gameplay.Fractions
{
    public class Fraction : NetworkBehaviour
    {
        [field: SerializeField] public string Name { get; private set; }
        [Space]
        [SerializeField] private FractionStatus defaultFractionStatus = FractionStatus.Neutral;
        [SerializeField] private FractionStatus fractionStatusForMembers = FractionStatus.Ally;
        [SerializeField] private Transform[] spawnPoints;
        [Space]
        [SerializeField] private Color mainColor;
        [SerializeField] private Color additiveColor;
        [Space(10f)]
        [SerializeField] private Color agressiveColor;
        [SerializeField] private Color passiveColor;
        [Space(10f)]
        [SerializeField] private Color reverseColor;
        [SerializeField] private Color reverseAdditiveColor;

        private Dictionary<Fraction, FractionStatus> statusDictionary;


        private uint currentSpawnPoint;



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



        [Command(requiresAuthority = false)]
        public void Join(NetworkConnectionToClient sender = null)
        {
            if (sender.identity == null)
                return;

            if (sender.identity.TryGetComponent<Entity>(out var entity) == false)
                return;

            if (CanJoin(entity) == false)
                return;



            entity.FindAbillity<AbillityFraction>().SetFraction(this);


            Transform spawnPoint = GetSpawnPoint();

            if (entity.TryGetComponent<Rigidbody>(out var rb))
                rb.Move(spawnPoint.position, spawnPoint.rotation);

            else
                entity.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }



        public Transform GetSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return this.transform;

            Transform spawnPoint = spawnPoints[currentSpawnPoint];
            currentSpawnPoint.IncreaseInBounds(spawnPoints);

            return spawnPoint;
        }

        public virtual Color GetColor(FractionColor fractionColor)
        {
            return fractionColor switch
            {
                FractionColor.Main => mainColor,
                FractionColor.Additive => additiveColor,
                FractionColor.Reverse => reverseColor,
                FractionColor.ReverseAdditive => reverseAdditiveColor,
                FractionColor.Agressive => agressiveColor,
                FractionColor.Passive => passiveColor,
                _ => mainColor
            };
        }



        protected virtual bool CanJoin(Entity entity)
        {
            if (entity.FindAbillity<AbillityFraction>().GetFraction() != null)
                return false;

            return true;
        }
    }
}
