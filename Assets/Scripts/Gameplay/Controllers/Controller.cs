using UnityEngine;
using Scripts.Gameplay.Entities;
using System.Collections.Generic;
using System.Linq;
using Scripts.Gameplay.Abillities;

namespace Scripts.Gameplay.Controllers
{
    /// <summary>
    /// Class for controlling abillities
    /// </summary>
    public abstract class Controller : MonoBehaviour
    {
        /// <summary>
        /// Initializes the controller with given entity
        /// </summary>
        public abstract void Initialize(Entity entity);


        /// <summary>
        /// Finds and gets as interfaces abillities from given entity
        /// </summary>
        public static IEnumerable<T> GetAsInterfaces<T>(IEnumerable<Abillity> abillities) where T : class
        {
            return from a in abillities
                   where a is T
                   select a as T;
        }
    }
}
