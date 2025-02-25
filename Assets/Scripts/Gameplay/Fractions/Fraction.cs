using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Fractions
{
    public class Fraction : NetworkBehaviour
    {
        [field: SerializeField] public string Name { get; protected set; }
        [Space]
        [SerializeField] protected FractionStatus defaultFractionStatus = FractionStatus.Neutral;
        [SerializeField] protected FractionStatus fractionStatusForMembers = FractionStatus.Ally;
        [Space]
        [SerializeField] private Color mainColor;
        [SerializeField] private Color additiveColor;
        [Space(10f)]
        [SerializeField] private Color agressiveColor;
        [SerializeField] private Color passiveColor;
        [Space(10f)]
        [SerializeField] private Color reverseColor;
        [SerializeField] private Color reverseAdditiveColor;

        protected readonly Dictionary<Fraction, FractionStatus> statusDictionary = new();
        protected readonly SyncHashSet<Entity> members = new();



        #region FractionStatus
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
        #endregion


        #region Requests To Join or Leave
        [Command(requiresAuthority = false)]
        public void RequestToJoin(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
                return;

            if (sender.identity == null)
                return;

            if (sender.identity.TryGetComponent<Entity>(out var entity) == false)
                return;


            Join(entity);
        }

        [Command(requiresAuthority = false)]
        public void RequestToLeave(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
                return;

            if (sender.identity == null)
                return;

            if (sender.identity.TryGetComponent<Entity>(out var entity) == false)
                return;


            Leave(entity);
        }
        #endregion

        #region Join or Leave
        [Server]
        public virtual bool Join(Entity entity)
        {
            if (CanJoin(entity) == false)
                return false;

            members.Add(entity);


            return true;
        }

        [Server]
        public virtual bool Leave(Entity entity)
        {
            if (CanLeave(entity) == false)
                return false;

            members.Remove(entity);


            return true;
        }
        #endregion

        #region Can Join or Leave
        protected virtual bool CanJoin(Entity entity)
        {
            return !members.Contains(entity);
        }

        protected virtual bool CanLeave(Entity entity)
        {
            return members.Contains(entity);
        }
        #endregion


        #region Gets
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


        public virtual IReadOnlyCollection<Entity> GetMembers() => members;
        #endregion
    }
}
