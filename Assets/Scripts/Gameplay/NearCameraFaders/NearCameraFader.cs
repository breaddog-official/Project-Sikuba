using Scripts.MonoCache;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Gameplay.NearCamera
{
    public class NearCameraFader : MonoBehaviour, IMonoCacheFixedUpdate
    {
        [SerializeField] protected LayerMask layerMask;

        protected readonly Dictionary<Transform, Dictionary<Transform, AlphaFader>> targets = new();

        public Behaviour Behaviour => this;


        private void Awake()
        {
            MonoCacher.Registrate(this);
        }

        public void FixedUpdateCached()
        {
            // Find new overlappers
            RaycastHit[] hits = null;

            foreach (var target in targets)
            {
                Vector3 direction = target.Key.position - transform.position;

                hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, layerMask);

                gDir = direction;
                gPos = transform.position;

                print(hits);
                if (hits == null)
                    continue;

                IEnumerable<Transform> hitsTransforms = hits.Select(r => r.transform);

                // Remove old overlappers
                foreach (var obstacle in target.Value.ToArray())
                {
                    if (!hitsTransforms.Contains(obstacle.Key))
                    {
                        target.Value.Remove(obstacle.Key);
                        obstacle.Value.Show();
                    }
                }

                // Find new overlappers
                foreach (var hit in hits)
                {
                    if (!target.Value.ContainsKey(hit.transform) && hit.transform.TryGetComponent<AlphaFader>(out var fader))
                    {
                        target.Value.Add(hit.transform, fader);
                        fader.Fade();
                    }
                }
            }
        }
        Vector3 gDir;
        Vector3 gPos;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(gPos, gDir);
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
