using System;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityCollisioner : Abillity
    {
        public event Action OnChangedGrounded;


        public abstract bool InAir();

        public abstract bool OnGround();


        protected virtual void ChangeGroundedInvoke()
        {
            OnChangedGrounded?.Invoke();
        }
    }
}