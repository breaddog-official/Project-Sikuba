using System;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityJump : Abillity
    {
        public virtual event Action OnJump;

        public virtual bool TryJump()
        {
            OnJump?.Invoke();
            return true;
        }

        /// <summary>
        /// Returns true if Entity currently can jump
        /// </summary>
        public abstract bool CanJump();
    }
}