using Unity.Cinemachine;
using Mirror;
using Scripts.MonoCache;
using NaughtyAttributes;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCamera : Abillity, IMonoCacheUpdate
    {
        enum FollowMode
        {
            Player,
            Manual
        }

        [SerializeField] private Transform target;
        [SerializeField] private FollowMode followMode;
        [Space]
        [SerializeField] private Vector3 offset;
        [SerializeField] private float rotationOffsetY = 45f;
        [SerializeField] private uint initialDirection;
        [SerializeField, MinValue(1)] private uint directionsCount;
        [Space]
        [SerializeField] private float followSpeed = 1f;
        [SerializeField] private float rotateSpeed = 1f;

        protected uint currentDirection;



        Vector3 targetOffset;
        Quaternion targetRotation;



        public Behaviour Behaviour => this;


        public override bool Initialize()
        {
            base.Initialize();

            targetOffset = MainCamera.Instance.CameraFollow.FollowOffset;
            targetRotation = MainCamera.Instance.CameraCinemachine.transform.rotation;

            currentDirection = initialDirection;

            MonoCacher.Registrate(this);

            ApplyDirection();

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
            if ((followMode == FollowMode.Player && !isLocalPlayer) || followMode == FollowMode.Manual)
                return;

            SetFollow(target);
        }

        [ClientCallback]
        protected override void OnDisable()
        {
            if ((followMode == FollowMode.Player && !isLocalPlayer) || followMode == FollowMode.Manual)
                return;

            SetFollow(null);
        }


        [ClientCallback]
        public void UpdateCached()
        {
            ApplyCameraTransforms(Time.deltaTime);
        }


        public virtual void SetFollow(Transform follow)
        {
            if (MainCamera.Instance != null && MainCamera.Instance.CameraCinemachine != null)
            {
                MainCamera.Instance.CameraCinemachine.Follow = follow;
            }
        }



        [Client]
        public virtual void TurnRight()
        {
            currentDirection.DecreaseInBounds(directionsCount, true);

            ApplyDirection();
        }

        [Client]
        public virtual void TurnLeft()
        {
            currentDirection.IncreaseInBounds(directionsCount, false);

            ApplyDirection();
        }



        protected virtual void ApplyDirection()
        {
            CinemachineFollow transposer = MainCamera.Instance.CameraFollow;
            Transform cameraTransform = MainCamera.Instance.CameraCinemachine.transform;
            Vector3 cameraRotationEulers = cameraTransform.rotation.eulerAngles;

            if (transposer == null || cameraTransform == null)
                return;



            float targetRotationY = 360 / directionsCount * currentDirection + rotationOffsetY;

            if (currentDirection == directionsCount)
            {
                currentDirection = 0;
                targetRotationY -= 360f;
            }

            targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, targetRotationY, cameraRotationEulers.z));



            /*float time = 1f / directionsCount * currentDirection;
            float tau = 2 * Mathf.PI;

            float xLerp = Mathf.Lerp(0.0f, tau, time);
            float zLerp = Mathf.Lerp(0.0f, tau, time);
            float xSin = Mathf.Cos(xLerp);
            float zSin = Mathf.Cos(zLerp);

            float targetVectorX = offset.x * xSin;
            float targetVectorZ = offset.z * zSin;

            print($"Sin X: {xSin}  Sin Z: {zSin} \nX: {targetVectorX}  Z: {targetVectorZ}");

            targetOffset.Set(targetVectorX, targetOffset.y, targetVectorZ);*/

            switch (currentDirection)
            {
                case 0: // Forward
                    targetOffset = offset;
                    break;

                case 2: // Backward
                    targetOffset = new Vector3(-offset.x, offset.y, -offset.z);
                    break;

                case 3: // Left
                    targetOffset = new Vector3(-offset.x, offset.y, offset.z);
                    break;

                case 1: // Right
                    targetOffset = new Vector3(offset.x, offset.y, -offset.z);
                    break;
            }
        }


        protected virtual void ApplyCameraTransforms(float deltaTime)
        {
            if (followMode == FollowMode.Player && !isLocalPlayer)
                return;

            CinemachineFollow transposer = MainCamera.Instance.CameraFollow;
            Transform cameraTransform = MainCamera.Instance.CameraCinemachine.transform;

            if (transposer == null || cameraTransform == null)
                return;

            transposer.FollowOffset = Vector3.Lerp(transposer.FollowOffset, targetOffset, followSpeed * deltaTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }
}