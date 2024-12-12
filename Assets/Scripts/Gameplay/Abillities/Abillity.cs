using Mirror;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Abillities
{
    public abstract class Abillity : NetworkBehaviour, IInitializable
    {
        public bool IsInitialized { get; protected set; }

        public Entity Entity { get; protected set; }

        /// <summary>
        /// Is available abillity for use?
        /// </summary>
        public virtual bool Available() => isActiveAndEnabled;

        public virtual bool Initialize() => IsInitialized = true;

        public virtual bool Initialize(Entity entity)
        {
            Entity = entity;
            return Initialize();
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
