using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.MonoCache
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

        private readonly static HashSet<IMonoCacheUpdate> listnersUpdate = new(CACHE_CAPACITY);
        private readonly static HashSet<IMonoCacheFixedUpdate> listnersFixedUpdate = new(CACHE_CAPACITY);
        private readonly static HashSet<IMonoCacheLateUpdate> listnersLateUpdate = new(CACHE_CAPACITY);

        /// <summary>
        /// Initial capacity of every cache
        /// </summary>
        private const int CACHE_CAPACITY = 32;


        public static void Registrate(IMonoCacheListner listner)
        {
            listnersUpdate.AddIfNotNull(listner as IMonoCacheUpdate);
            listnersFixedUpdate.AddIfNotNull(listner as IMonoCacheFixedUpdate);
            listnersLateUpdate.AddIfNotNull(listner as IMonoCacheLateUpdate);
        }

        /// <summary>
        /// You don't need call this in OnDestroy. MonoCacher automatically removing destroyed or null listners
        /// </summary>
        public static void UnRegistrate(IMonoCacheListner listner)
        {
            listnersUpdate.Remove(listner as IMonoCacheUpdate);
            listnersFixedUpdate.Remove(listner as IMonoCacheFixedUpdate);
            listnersLateUpdate.Remove(listner as IMonoCacheLateUpdate);
        }



        private void Update()
        {
            RemoveNulls(RemoveTimings.Update);

            foreach (IMonoCacheUpdate updateListner in listnersUpdate)
            {
                // We skip null checks because we already removed all null listners in RemoveNulls
                if (!IsValid(updateListner, true))
                    continue;

                try
                {
                    updateListner.UpdateCached();
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                }
            }
        }

        private void FixedUpdate()
        {
            RemoveNulls(RemoveTimings.FixedUpdate);

            foreach (IMonoCacheFixedUpdate fixedUpdateListner in listnersFixedUpdate)
            {
                // We skip null checks because we already removed all null listners in RemoveNulls
                if (!IsValid(fixedUpdateListner, true))
                    continue;

                try
                {
                    fixedUpdateListner.FixedUpdateCached();
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                }
            }
        }

        private void LateUpdate()
        {
            RemoveNulls(RemoveTimings.LateUpdate);

            foreach (IMonoCacheLateUpdate lateUpdateListner in listnersLateUpdate)
            {
                // We skip null checks because we already removed all null listners in RemoveNulls
                if (!IsValid(lateUpdateListner, true))
                    continue;

                try
                {
                    lateUpdateListner.LateUpdateCached();
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                }
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
        private static bool IsValid(IMonoCacheListner listner, bool skipNullCheck = false)
                            => (skipNullCheck || !IsNull(listner)) && listner.Behaviour.isActiveAndEnabled;

        /// <summary>
        /// Checks if null
        /// </summary>
        private static bool IsNull(IMonoCacheListner listner)
                            => listner == null || listner.Behaviour == null;
    }
}
