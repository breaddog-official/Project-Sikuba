using UnityEngine;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Controllers
{
    /// <summary>
    /// Class for controlling abillities
    /// </summary>
    public abstract class Controller : MonoBehaviour, IInitializable
    {
        public Entity Entity { get; protected set; }

        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes the controller with Entity from GameObject
        /// </summary>
        public virtual void Initialize()
        {
            if (gameObject.TryGetComponent(out Entity entity))
            {
                Initialize(entity);
            }
        }

        /// <summary>
        /// Initializes the controller with given entity
        /// </summary>
        public virtual void Initialize(Entity entity)
        {
            Entity = entity;
            IsInitialized = true;
        }
    }
}
