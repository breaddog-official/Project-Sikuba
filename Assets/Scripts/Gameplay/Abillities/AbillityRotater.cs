using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityRotater : Abillity
    {
        public abstract void RotateToPoint(Vector2 point);
        public abstract void Rotate(Vector3 vector);

        /// <summary>
        /// Returns true if Rotate must be called only in FixedUpdate
        /// </summary>
        public abstract bool IsPhysicsRotater();
    }
}
