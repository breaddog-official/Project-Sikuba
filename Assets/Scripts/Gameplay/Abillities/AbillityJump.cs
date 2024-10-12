using System;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityJump : Abillity
    {
        public virtual event Action OnJump;

        public virtual void Jump()
        {
            OnJump?.Invoke();
        }
    }
}