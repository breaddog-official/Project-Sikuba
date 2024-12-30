using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Scripts.MonoCache;
using Scripts.Gameplay.Abillities;
using NaughtyAttributes;
using System;

namespace Scripts.Network
{
    [AddComponentMenu("Network/ Interest Management/ Prediction/Prediction Interest Management")]
    public class PredictionInterestManagement : InterestManagement, IMonoCacheUpdate
    {
        [Flags]
        enum VisibleBehaviour
        {
            None = 0,
            Distance = 1 << 0,
            Linecast = 1 << 1,
            Prediction = 1 << 2,
        }

        [Min(0f)]
        [Header("Interest Management")]
        [SerializeField] private float maxDistance = 25f;
        [SerializeField, EnumFlags] private VisibleBehaviour defaultBehaviour;
        [SerializeField, EnumFlags] private VisibleBehaviour sameFractionBehaviour;
        [Header("Prediction")]
        [SerializeField] private int raysCount = 64;
        [SerializeField] private float raysSpace = 0.25f;
        [SerializeField] private float maxPredictionAngle = 90f;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private bool drawGizmos;
        [Space, Min(1)]
        [SerializeField] private uint rebuildEveryFrames = 2;

        private uint currentRebuildFrame;
        private Vector3 gizmosIdentity;
        private Vector3 gizmosDirectionToSecond;
        private Vector3 gizmosDirectionToWall;

        private readonly Dictionary<NetworkIdentity, AbillityDataFraction> dataFractions = new();



        public Behaviour Behaviour => this;





        protected virtual void Start()
        {
            MonoCacher.Registrate(this);
        }

        [ServerCallback]
        public void UpdateCached()
        {
            // rebuild all spawned NetworkIdentity's observers every 'rebuildEveryFrames'
            if (++currentRebuildFrame == rebuildEveryFrames)
            {
                currentRebuildFrame = 0;
                RebuildAll();
            }
        }

        [ServerCallback]
        public override void ResetState()
        {
            currentRebuildFrame = 0;
            dataFractions.Clear();
        }


        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            // authenticated and joined world with a player?
            if (newObserver != null && newObserver.isAuthenticated && newObserver.identity != null)
            {
                return IsVisible(newObserver.identity, identity);
            }
            else
            {
                return false;
            }
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (OnCheckObserver(identity, conn))
                {
                    newObservers.Add(conn);
                }
            }
        }


        public override void OnSpawned(NetworkIdentity identity)
        {
            if (identity.TryGetComponent<AbillityDataFraction>(out var dataFraction))
            {
                dataFractions.Add(identity, dataFraction);
            }
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            dataFractions.Remove(identity);
        }



        public bool IsVisible(NetworkIdentity identity, NetworkIdentity observer)
        {
            Transform identityTransform = identity.transform;
            Transform observerTranform = observer.transform;

            VisibleBehaviour behaviour = defaultBehaviour;

            if (behaviour != sameFractionBehaviour && IsSameFraction(identity, observer))
                behaviour = sameFractionBehaviour;

            // Check distance
            if (behaviour.HasFlag(VisibleBehaviour.Distance) && !VisibleByDistance(identityTransform, observerTranform))
                return false;

            // Check linecast
            if (behaviour.HasFlag(VisibleBehaviour.Linecast) && VisibleByLinecast(identityTransform, observerTranform))
                return true;

            // Check prediction
            if (behaviour.HasFlag(VisibleBehaviour.Prediction) && !VisibleByPrediction(identityTransform, observerTranform))
                return false;

            return true;
        }


        #region Checks

        public bool IsSameFraction(NetworkIdentity identity, NetworkIdentity observer)
        {
            if (dataFractions.TryGetValue(observer, out var identityData) &&
                dataFractions.TryGetValue(identity, out var observerData))
            {
                if (identityData.Get() == observerData.Get())
                    return true;
            }
            return false;
        }

        public bool VisibleByDistance(Transform first, Transform second)
        {
            print("distance");
            return Vector3.Distance(first.position, second.position) > maxDistance;
        }

        public bool VisibleByLinecast(Transform first, Transform second)
        {
            print("linecast");
            return !Physics.Linecast(first.position, second.position, raycastLayerMask);
        }

        public bool VisibleByPrediction(Transform first, Transform second)
        {
            print("prediction");
            Vector3 directionToSecond = second.position - first.position;
            Vector3? directionToWall = null;

            // Same as Vector3.Distance
            float distance = directionToSecond.magnitude;

            for (int i = 0; i < raysCount; i++)
            {
                Vector3 eulers = Quaternion.LookRotation(directionToSecond).eulerAngles;
                Vector3 direction = Quaternion.Euler(eulers.x, eulers.y + (i % 2 == 0 ? raysSpace : -raysSpace) * i, eulers.z) * Vector3.forward;

                if (!Physics.Raycast(first.position, direction, distance, raycastLayerMask))
                {
                    directionToWall = direction;
                    break;
                }
            }

            if (!directionToWall.HasValue)
                return false;


            if (drawGizmos)
            {
                gizmosIdentity = first.position;
                gizmosDirectionToSecond = directionToSecond;
                gizmosDirectionToWall = directionToWall.Value;
            }

            return Vector3.Angle(directionToSecond, directionToWall.Value) > maxPredictionAngle;
        }

        #endregion

        #region Debug

        
        protected virtual void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(gizmosIdentity, gizmosDirectionToSecond);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(gizmosIdentity, gizmosDirectionToWall);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmosDirectionToSecond, gizmosDirectionToWall);
        }

        #endregion
    }
}