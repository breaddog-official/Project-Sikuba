using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Abillities
{
    public abstract class Abillity : NetworkBehaviour, IInitializable, IResetable
    {
        protected bool isInitialized;

        public bool IsInitialized => isInitialized;

        public Entity Entity { get; protected set; }

        /// <summary>
        /// Is available abillity for use?
        /// </summary>
        public virtual bool Available() => isActiveAndEnabled;

        public virtual bool Initialize()
        {
            if (isInitialized.CheckInitialization())
                return false;

            return IsInitialized;
        }

        public virtual bool Initialize(Entity entity)
        {
            Entity = entity;
            return Initialize();
        }

        public virtual void ResetState()
        {
        }


        // Needed for the checkbox in the inspector

        /// <summary>
        /// If you override this, you don't need to write <see href="base.OnEnable"/>
        /// </summary>
        protected virtual void OnEnable() { }
        /// <summary>
        /// If you override this, you don't need to write <see href="base.OnDisable"/>
        /// </summary>
        protected virtual void OnDisable() { }
    }
}
