using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Scripts.MonoCache;
using System.Linq;

namespace Scripts.Network
{
    [AddComponentMenu("Network/ Interest Management/ Prediction/Prediction Interest Management")]
    public class PredictionInterestManagement : InterestManagement, IMonoCacheUpdate
    {
        [Min(0f)]
        [SerializeField] private int raysCount = 64;
        [SerializeField] private float raysSpace = 0.25f;
        [SerializeField] private float maxAngle = 90f;
        [SerializeField] private float maxDistance = 25f;
        [SerializeField] private float projectMaxDistance = 3f;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private bool drawGizmos;
        [Space, Min(1)]
        [SerializeField] private uint rebuildEveryFrames = 2;
        private uint currentRebuildFrame;


        public Behaviour Behaviour => this;


        protected virtual void Start()
        {
            MonoCacher.Registrate(this);
        }

        [ServerCallback]
        public override void ResetState()
        {
            currentRebuildFrame = 0;
        }


        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            // authenticated and joined world with a player?
            if (newObserver != null && newObserver.isAuthenticated && newObserver.identity != null)
            {
                return Predict(newObserver.identity.transform, identity.transform);
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



        public bool Predict(Transform observer, Transform gameObject)
        {
            // Check distance
            float distance = Vector3.Distance(observer.position, gameObject.position);
            if (distance > maxDistance)
                return false;

            bool linecast = Physics.Linecast(observer.position, gameObject.position, out RaycastHit raycastObject, raycastLayerMask);

            // Check linecast
            if (!linecast)
                return true;

            Vector3 directionToObject = gameObject.position - observer.position;
            Vector3? directionToWall = null;

            for (int i = 0; i < raysCount; i++)
            {
                Vector3 eulers = Quaternion.LookRotation(directionToObject).eulerAngles;
                Vector3 direction = Quaternion.Euler(eulers.x, eulers.y + (i % 2 == 0 ? raysSpace : -raysSpace) * i, eulers.z) * Vector3.forward;

                if (!Physics.Raycast(observer.position, direction, distance, raycastLayerMask))
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
            Vector3 worldProject = observer.position + project;

            if (drawGizmos)
            {
                gizmosObserver = observer;
                gizmosGameObject = gameObject;
                gizmosWorldProject = worldProject;
                gizmosDirectionToObject = directionToObject;
                gizmosDirectionToWall = directionToWall;
            }

            return Vector3.Distance(worldProject, gameObject.position) < projectMaxDistance;
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