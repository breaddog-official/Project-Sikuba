using Scripts.MonoCache;
using TMPro;
using UnityEngine;

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
                cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.position + MainCamera.Instance.CameraTransposer.m_FollowOffset - cachedTransform.position);
            }
        }

        protected override void SetNickname(string nickname)
        {
            text.SetText(nickname);
        }
    }
}
