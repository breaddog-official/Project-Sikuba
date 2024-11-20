using Cysharp.Threading.Tasks;
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
        enum RemoveTimings
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

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
            RemoveNulls(RemoveTimings.Update);

            foreach (IMonoCacheUpdate updateListner in listnersUpdate)
            {
                if (!IsValid(updateListner, true))
                    continue;

                updateListner.UpdateCached();
            }
        }

        private void FixedUpdate()
        {
            RemoveNulls(RemoveTimings.FixedUpdate);

            foreach (IMonoCacheFixedUpdate fixedUpdateListner in listnersFixedUpdate)
            {
                if (!IsValid(fixedUpdateListner, true))
                    continue;

                fixedUpdateListner.FixedUpdateCached();
            }
        }

        private void LateUpdate()
        {
            RemoveNulls(RemoveTimings.LateUpdate);

            foreach (IMonoCacheLateUpdate lateUpdateListner in listnersLateUpdate)
            {
                if (!IsValid(lateUpdateListner, true))
                    continue;

                lateUpdateListner.LateUpdateCached();
            }
        }


        private void RemoveNulls(RemoveTimings timing)
        {
            switch (timing)
            {
                case RemoveTimings.Update:
                    listnersUpdate.RemoveWhere(IsNull);
                    return;

                case RemoveTimings.FixedUpdate:
                    listnersFixedUpdate.RemoveWhere(IsNull);
                    return;

                case RemoveTimings.LateUpdate:
                    listnersLateUpdate.RemoveWhere(IsNull);
                    return;
            }
        }


        /// <summary>
        /// Checks if not null and active
        /// </summary>
        private bool IsValid(IMonoCacheListner listner, bool skipNullCheck = false)
                            => (skipNullCheck || !IsNull(listner)) && listner.Behaviour.isActiveAndEnabled;

        /// <summary>
        /// Checks if null
        /// </summary>
        private bool IsNull(IMonoCacheListner listner) 
                            => listner == null || listner.Behaviour == null;
    }
}
