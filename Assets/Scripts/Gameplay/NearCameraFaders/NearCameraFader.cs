using Scripts.MonoCache;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Gameplay.CameraManagement
{
    public class NearCameraFader : CameraEffect, IMonoCacheFixedUpdate
    {
        [SerializeField] protected LayerMask layerMask;

        /// <summary>
        /// Dictionary that contains: <br />
        /// 1. Target transform <br />
        /// 2. Obstacles set
        /// </summary>
        protected readonly Dictionary<Transform, HashSet<Transform>> targets = new();
        protected readonly Dictionary<Transform, Fader> fadersCache = new();

        public Behaviour Behaviour => this;


        private void Awake()
        {
            MonoCacher.Registrate(this);
        }

        public void FixedUpdateCached()
        {
            // Foreach all observers (like Players, bullets)
            foreach (var target in targets)
            {
                Vector3 direction = target.Key.position - transform.position;

                RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, layerMask);


                IEnumerable<Transform> hitsTransforms = hits?.Select(r => r.transform);

                // Remove old overlappers

                // Foreach all overlapping obstacles (like walls)
                foreach (var obstacle in target.Value.ToArray())
                {
                    if (hitsTransforms == null || !hitsTransforms.Contains(obstacle))
                    {
                        target.Value.Remove(obstacle);

                        if (TryGetFader(obstacle, out var fader))
                            fader.Show();
                    }
                }

                // Find new overlappers

                if (hitsTransforms == null)
                    continue;

                // Foreach all unregistred obstacles from raycast
                foreach (var hitTransform in hitsTransforms)
                {
                    if (!target.Value.Contains(hitTransform))
                    {
                        target.Value.Add(hitTransform);

                        if (TryGetFader(hitTransform, out var fader))
                            fader.Fade();
                    }
                }
            }
        }


        public void AddTarget(Transform target)
        {
            targets.Add(target, new());
        }

        public void RemoveTarget(Transform target)
        {
            targets.Remove(target);
        }


        private bool TryGetFader(Transform transform, out Fader fader)
        {
            if (!fadersCache.TryGetValue(transform, out fader))
            { 
                fader = transform.GetComponent<Fader>() ?? transform.GetComponentInParent<Fader>() ?? transform.GetComponentInChildren<Fader>();

                if (fader != null)
                {
                    fadersCache.Add(transform, fader);
                }
            }

            return fader != null;
        }
    }
}
