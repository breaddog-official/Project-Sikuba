using System;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityMove : Abillity
    {
        /// <summary>
        /// Sends local space velocity of move
        /// </summary>
        public virtual event Action<Vector3> OnMove;
        public virtual event Action OnStartMove;
        public virtual event Action OnStopMove;

        public virtual void Move(Vector3 vector)
        {
            OnMove?.Invoke(vector);
        }

        public virtual void StartMove()
        {
            OnStartMove?.Invoke();
        }

        public virtual void StopMove()
        {
            OnStopMove?.Invoke();
        }

        /// <summary>
        /// Returns true if Move must be called only in FixedUpdate
        /// </summary>
        public abstract bool IsPhysicsMovement();
    }
}