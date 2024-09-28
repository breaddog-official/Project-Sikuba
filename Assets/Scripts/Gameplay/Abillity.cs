using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public abstract class Abillity : MonoBehaviour
    {
        /// <summary>
        /// Is available abillity for using?
        /// </summary>
        public virtual bool Available() => isActiveAndEnabled;
    }
}
