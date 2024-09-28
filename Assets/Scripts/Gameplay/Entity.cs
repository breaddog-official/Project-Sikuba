using System.Collections.Generic;
using UnityEngine;
using Scripts.Gameplay.Controllers;
using Scripts.Gameplay.Abillities;

namespace Scripts.Gameplay.Entities
{
    /// <summary>
    /// Class for managing controlles and updating abillities
    /// </summary>
    public class Entity : MonoBehaviour
    {
        #region Fields && Properties

        private Controller controller;

        private Abillity[] abillities;




        public Controller Controller => controller;

        public IReadOnlyCollection<Abillity> Abillities => abillities;

        #endregion

        /// <summary>
        /// Initializes the entity
        /// </summary>
        public virtual void Initialize()
        {
            // Find all abillities
            abillities = GetComponents<Abillity>();

            // Set initial controller if exists
            if (TryGetComponent<Controller>(out var initialController))
                SetController(initialController);
        }



        /// <summary>
        /// Adds the controller and sets it to entity
        /// </summary>
        public virtual bool SetController<T>() where T : Controller
        {
            T addedController = gameObject.AddComponent<T>();
            return SetController(addedController);
        }

        /// <summary>
        /// Sets existing controller to entity
        /// </summary>
        public virtual bool SetController(Controller controller)
        {
            if (controller == null || this.controller == controller)
                return false;

            // Remove component because cant be two controllers
            if (this.controller != null)
                Destroy(this.controller);

            this.controller = controller;
            this.controller.Initialize(this);

            return true;
        }
    }
}