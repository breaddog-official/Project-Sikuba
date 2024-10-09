using System.Collections.Generic;
using UnityEngine;
using Scripts.Gameplay.Controllers;
using Scripts.Gameplay.Abillities;
using Mirror;
using System.Linq;
using System;
using Scripts.Extensions;

namespace Scripts.Gameplay.Entities
{
    /// <summary>
    /// Class for managing controlles and updating abillities
    /// </summary>
    public class Entity : NetworkBehaviour, IInitializable
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

        /// <summary>
        /// Finds abillity by T
        /// </summary>
        public T FindAbillity<T>() where T : Abillity
        {
            //return Abillities.Where(a => a is T)
            //                 .FirstOrDefault() as T;

            return Abillities.FindByType<T>();
        }
        /// <summary>
        /// Finds abillity by type
        /// </summary>
        public Abillity FindAbillity(Type type)
        {
            //return Abillities.Where(a => a.GetType() == type)
            //                 .FirstOrDefault();

            return Abillities.FindByType(type) as Abillity;
        }
    }
}