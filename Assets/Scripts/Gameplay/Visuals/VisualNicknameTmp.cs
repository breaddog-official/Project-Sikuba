using Scripts.MonoCache;
using TMPro;
using UnityEngine;
using Scripts.Gameplay.CameraManagement;

namespace Scripts.Gameplay.Visuals
{
    public class VisualNicknameTmp : VisualNickname, IMonoCacheUpdate
    {
        [SerializeField] protected TextMeshPro text;
        [SerializeField] protected bool rotateToCamera = true;

        protected Transform cachedTransform;

        public Behaviour Behaviour => this;


        private void Awake()
        {
            MonoCacher.Registrate(this);
            cachedTransform = transform;
        }

        public virtual void UpdateCached()
        {
            if (rotateToCamera)
            {
                Vector3 direction = cachedTransform.position + MainCamera.Instance.CinemachineFollow.FollowOffset - cachedTransform.position;
                cachedTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        protected override void SetNickname(string nickname)
        {
            text.SetText(nickname);
        }
    }
}
