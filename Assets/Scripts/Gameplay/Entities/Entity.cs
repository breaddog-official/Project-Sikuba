using System.Collections.Generic;
using UnityEngine;
using Scripts.Gameplay.Controllers;
using Scripts.Gameplay.Abillities;
using Mirror;
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

        [SerializeField] private bool initializeIfLocalPlayer = true;


        private Controller controller;

        private Abillity[] abillities;



        public bool IsInitialized { get; private set; }

        public Controller Controller => controller;

        public IReadOnlyCollection<Abillity> Abillities => abillities;

        #endregion

        public override void OnStartLocalPlayer()
        {
            if (initializeIfLocalPlayer)
                Initialize();
        }


        /// <summary>
        /// Initializes the entity
        /// </summary>
        public virtual bool Initialize()
        {
            if (IsInitialized)
                return false;

            IsInitialized = true;


            // Find all abillities
            abillities = GetComponents<Abillity>();

            // Initialize all abillities
            foreach (Abillity abillity in abillities)
            {
                abillity.Initialize(this);
            }

            // Set initial controller if exists
            if (TryGetComponent<Controller>(out var initialController))
                SetController(initialController);

            return true;
        }


        #region Controller

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

        #endregion

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



        public bool TryFindAbillity<T>(out T abillity) where T : Abillity
        {
            abillity = FindAbillity<T>();

            return abillity != null;
        }

        public bool TryFindAbillity(out Abillity abillity, Type type)
        {
            abillity = FindAbillity(type);

            return abillity != null;
        }
    }
}