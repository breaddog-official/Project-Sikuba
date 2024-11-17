using Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.MonoCacher
{
    /// <summary>
    /// Class for caching default unity events, since they use reflection by default
    /// </summary>
    public sealed class MonoCacher : MonoBehaviour
    {
        private readonly static HashSet<IMonoCacheUpdate> listnersUpdate = new(CACHE_CAPACITY / 3);
        private readonly static HashSet<IMonoCacheFixedUpdate> listnersFixedUpdate = new(CACHE_CAPACITY / 3);
        private readonly static HashSet<IMonoCacheLateUpdate> listnersLateUpdate = new(CACHE_CAPACITY / 3);

        /// <summary>
        /// Initial capacity of all caches <br /> <br />
        /// </summary>
        private const int CACHE_CAPACITY = 384; // ~3 kb


        public static void Registrate(IMonoCacheListner listner)
        {
            listnersUpdate.AddIfNotNull(listner as IMonoCacheUpdate, true);
            listnersFixedUpdate.AddIfNotNull(listner as IMonoCacheFixedUpdate, true);
            listnersLateUpdate.AddIfNotNull(listner as IMonoCacheLateUpdate, true);
        }



        private void Update()
        {
            listnersUpdate.RemoveWhere(m => m == null || m.Behaviour == null);

            foreach (IMonoCacheUpdate updateListner in listnersUpdate.Where(l => l.Behaviour.isActiveAndEnabled))
            {
                updateListner.UpdateCached();
            }
        }

        private void FixedUpdate()
        {
            listnersFixedUpdate.RemoveWhere(m => m == null || m.Behaviour == null);

            foreach (IMonoCacheFixedUpdate fixedUpdateListner in listnersFixedUpdate.Where(l => l.Behaviour.isActiveAndEnabled))
            {
                fixedUpdateListner.FixedUpdateCached();
            }
        }

        private void LateUpdate()
        {
            listnersLateUpdate.RemoveWhere(m => m == null || m.Behaviour == null);

            foreach (IMonoCacheLateUpdate lateUpdateListner in listnersLateUpdate.Where(l => l.Behaviour.isActiveAndEnabled))
            {
                lateUpdateListner.LateUpdateCached();
            }
        }
    }
}
