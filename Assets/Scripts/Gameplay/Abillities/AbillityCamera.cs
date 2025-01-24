using Unity.Cinemachine;
using Mirror;
using Scripts.MonoCache;
using NaughtyAttributes;
using UnityEngine;
using Scripts.Extensions;
using Scripts.Gameplay.CameraManagement;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCamera : Abillity
    {
        enum FollowMode
        {
            Player,
            Manual
        }

        [SerializeField] private Transform target;
        [SerializeField] private FollowMode followMode;
        [Space]
        [SerializeField] private bool enableNearCameraFading;


        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            return true;
        }

        public override void OnStartLocalPlayer()
        {
            if (followMode == FollowMode.Manual)
                return;

            SetFollow(target);
        }

        [ClientCallback]
        protected override void OnEnable()
        {
            if (enableNearCameraFading)
                MainCamera.Instance.GetCameraEffect<NearCameraFader>().AddTarget(target);

            if ((followMode == FollowMode.Player && !isLocalPlayer) || followMode == FollowMode.Manual)
                return;

            SetFollow(target);
        }

        [ClientCallback]
        protected override void OnDisable()
        {
            if(enableNearCameraFading)
                MainCamera.Instance.GetCameraEffect<NearCameraFader>().RemoveTarget(target);

            if ((followMode == FollowMode.Player && !isLocalPlayer) || followMode == FollowMode.Manual)
                return;

            SetFollow(null);
        }




        public virtual void SetFollow(Transform follow)
        {
            if (MainCamera.Instance != null && MainCamera.Instance.CinemachineCamera != null)
            {
                MainCamera.Instance.CinemachineCamera.Follow = follow;
            }
        }
    }
}