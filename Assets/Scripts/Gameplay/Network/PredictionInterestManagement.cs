using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Scripts.MonoCache;
using System.Linq;
using Scripts.Gameplay.Abillities;

namespace Scripts.Network
{
    [AddComponentMenu("Network/ Interest Management/ Prediction/Prediction Interest Management")]
    public class PredictionInterestManagement : InterestManagement, IMonoCacheUpdate
    {
        [Min(0f)]
        [Header("Interest Management")]
        [SerializeField] private float maxDistance = 25f;
        [SerializeField] private bool dontPredictSameFraction = true;
        [Header("Prediction")]
        [SerializeField] private int raysCount = 64;
        [SerializeField] private float raysSpace = 0.25f;
        [SerializeField] private float maxAngle = 90f;
        [SerializeField] private float projectMaxDistance = 3f;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private bool drawGizmos;
        [Space, Min(1)]
        [SerializeField] private uint rebuildEveryFrames = 2;
        private uint currentRebuildFrame;

        private readonly Dictionary<NetworkIdentity, AbillityDataFraction> dataFractions = new();

        public Behaviour Behaviour => this;


        protected virtual void Start()
        {
            MonoCacher.Registrate(this);
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
                return Predict(newObserver.identity, identity);
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
            if (dontPredictSameFraction && identity.TryGetComponent<AbillityDataFraction>(out var dataFraction))
            {
                dataFractions.Add(identity, dataFraction);
            }
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            if (dontPredictSameFraction)
                dataFractions.Remove(identity);
        }



        public bool Predict(NetworkIdentity identity, NetworkIdentity observer)
        {
            Transform observerTranform = observer.transform;
            Transform identityTransform = identity.transform;

            // Check distance
            float distance = Vector3.Distance(observerTranform.position, identityTransform.position);
            if (distance > maxDistance)
                return false;

            if (dontPredictSameFraction && dataFractions.TryGetValue(observer, out var identityData) && 
                                           dataFractions.TryGetValue(identity, out var observerData))
            {
                if (identityData.Get() == observerData.Get())
                    return true;
            }

            // Prediction start
            bool linecast = Physics.Linecast(observerTranform.position, identityTransform.position, out RaycastHit raycastObject, raycastLayerMask);

            // Check linecast
            if (!linecast)
                return true;

            Vector3 directionToObject = identityTransform.position - observerTranform.position;
            Vector3? directionToWall = null;

            for (int i = 0; i < raysCount; i++)
            {
                Vector3 eulers = Quaternion.LookRotation(directionToObject).eulerAngles;
                Vector3 direction = Quaternion.Euler(eulers.x, eulers.y + (i % 2 == 0 ? raysSpace : -raysSpace) * i, eulers.z) * Vector3.forward;

                if (!Physics.Raycast(observerTranform.position, direction, distance, raycastLayerMask))
                {
                    directionToWall = direction;
                    break;
                }
            }

            if (!directionToWall.HasValue)
                return false;

            // Check angle
            if (Vector3.Angle(directionToObject, directionToWall.Value) > maxAngle)
                return false;


            Vector3 project = Vector3.Project(directionToObject, directionToWall.Value);
            Vector3 worldProject = observerTranform.position + project;

            if (drawGizmos)
            {
                gizmosObserver = observerTranform;
                gizmosGameObject = identityTransform;
                gizmosWorldProject = worldProject;
                gizmosDirectionToObject = directionToObject;
                gizmosDirectionToWall = directionToWall;
            }

            return Vector3.Distance(worldProject, identityTransform.position) < projectMaxDistance;
        }



        Transform gizmosObserver;
        Transform gizmosGameObject;
        Vector3? gizmosWorldProject;
        Vector3? gizmosDirectionToObject;
        Vector3? gizmosDirectionToWall;
        protected virtual void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            if (gizmosObserver == null || gizmosGameObject == null ||
                !gizmosWorldProject.HasValue || !gizmosDirectionToObject.HasValue || !gizmosDirectionToWall.HasValue)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmosObserver.position, gizmosWorldProject.Value);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(gizmosObserver.position, gizmosDirectionToObject.Value);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(gizmosObserver.position, gizmosDirectionToWall.Value);
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
    }
}