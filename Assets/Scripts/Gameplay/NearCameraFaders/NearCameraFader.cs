using Scripts.MonoCache;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Gameplay.NearCamera
{
    public class NearCameraFader : MonoBehaviour, IMonoCacheFixedUpdate
    {
        [SerializeField] protected LayerMask layerMask;

        /// <summary>
        /// Dictionary that contains: <br />
        /// 1. Target transform <br />
        /// 2. Obstacles Dictionary that contains: <br />
        /// 2.1. Obstacle transform <br />
        /// 2.2. Obstacle fader <br />
        /// </summary>
        protected readonly Dictionary<Transform, Dictionary<Transform, Fader>> targets = new();


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
                    if (hitsTransforms == null || !hitsTransforms.Contains(obstacle.Key))
                    {
                        target.Value.Remove(obstacle.Key);
                        obstacle.Value.Show();
                    }
                }

                // Find new overlappers

                // Foreach all unregistred obstacles from raycast
                foreach (var hit in hits)
                {
                    if (!target.Value.ContainsKey(hit.transform) && hit.transform.TryGetComponent<Fader>(out var fader))
                    {
                        target.Value.Add(hit.transform, fader);
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
    }
}
