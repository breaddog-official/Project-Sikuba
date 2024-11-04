using Scripts.Extensions;
using System.Collections.Generic;
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
            foreach (IMonoCacheUpdate updateListner in listnersUpdate)
            {
                if (updateListner == null || updateListner.Behaviour == null)
                {
                    listnersUpdate.Remove(updateListner);
                    continue;
                }
                if (updateListner.Behaviour.isActiveAndEnabled == false)
                {
                    continue;
                }

                updateListner.UpdateCached();
            }
        }

        private void FixedUpdate()
        {
            foreach (IMonoCacheFixedUpdate fixedUpdateListner in listnersFixedUpdate)
            {
                if (fixedUpdateListner == null || fixedUpdateListner.Behaviour == null)
                {
                    listnersFixedUpdate.Remove(fixedUpdateListner);
                    continue;
                }
                if (fixedUpdateListner.Behaviour.isActiveAndEnabled == false)
                {
                    continue;
                }

                fixedUpdateListner.FixedUpdateCached();
            }
        }

        private void LateUpdate()
        {
            foreach (IMonoCacheLateUpdate lateUpdateListner in listnersLateUpdate)
            {
                if (lateUpdateListner == null || lateUpdateListner.Behaviour == null)
                {
                    listnersLateUpdate.Remove(lateUpdateListner);
                    continue;
                }
                if (lateUpdateListner.Behaviour.isActiveAndEnabled == false)
                {
                    continue;
                }

                lateUpdateListner.LateUpdateCached();
            }
        }
    }
}
