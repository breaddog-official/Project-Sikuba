using System;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityMove : Abillity
    {
        public virtual event Action<Vector3> OnMove;

        public virtual void Move(Vector3 vector)
        {
            OnMove?.Invoke(vector);
        }

        /// <summary>
        /// Returns true if Move must be called only in FixedUpdate
        /// </summary>
        public abstract bool IsPhysicsMovement();
    }
}