using Cinemachine;
using Mirror;
using Scripts.MonoCache;
using NaughtyAttributes;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCamera : Abillity, IMonoCacheUpdate
    {
        [SerializeField] private Transform target;
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

            targetOffset = MainCamera.Instance.CameraTransposer.m_FollowOffset;
            targetRotation = MainCamera.Instance.VirtualCamera.transform.rotation;

            currentDirection = initialDirection;

            MonoCacher.Registrate(this);

            SetFollow(true);
            ApplyDirection();

            return true;
        }

        [ClientCallback]
        protected override void OnEnable()
        {
            if (!isOwned)
                return;

            SetFollow(true);
        }

        [ClientCallback]
        protected override void OnDisable()
        {
            if (!isOwned)
                return;

            SetFollow(false);
        }


        [ClientCallback]
        public void UpdateCached()
        {
            if (!isOwned)
                return;

            ApplyCameraTransforms(Time.deltaTime);
        }


        protected virtual void SetFollow(bool state)
        {
            if (MainCamera.Instance != null && MainCamera.Instance.VirtualCamera != null)
            {
                MainCamera.Instance.VirtualCamera.Follow = state ? target : null;
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
            CinemachineTransposer transposer = MainCamera.Instance.CameraTransposer;
            Transform cameraTransform = MainCamera.Instance.VirtualCamera.transform;
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



            float time = 1f / directionsCount * currentDirection;
            float tau = 2 * Mathf.PI;

            float xLerp = Mathf.Lerp(0.0f, tau, time);
            float zLerp = Mathf.Lerp(0.0f, tau, time);
            float xSin = Mathf.Cos(xLerp);
            float zSin = Mathf.Cos(zLerp);


            //Vector3 modifiedOffset = time < 0.5f ? offset : -offset;

            float targetVectorX = offset.x * xSin;
            float targetVectorZ = offset.z * zSin;

            print($"Sin Z: {zSin} \n X: {targetVectorX} \n Z: {targetVectorZ}");

            targetOffset.Set(targetVectorX, targetOffset.y, targetVectorZ);

            switch (currentDirection)
            {
                case 0: // Forward
                    //targetOffset = offset;
                    //targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, offsetYRotation, cameraRotationEulers.z));
                    break;

                case 1: // Backward
                    //targetOffset = new Vector3(-offset.x, offset.y, -offset.z);
                    //targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, -(90 + offsetYRotation), cameraRotationEulers.z));
                    break;

                case 2: // Right
                    //targetOffset = new Vector3(-offset.x, offset.y, offset.z);
                    //targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, -offsetYRotation, cameraRotationEulers.z));
                    break;

                case 3: // Left
                    //targetOffset = new Vector3(offset.x, offset.y, -offset.z);
                    //targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, 90 + offsetYRotation, cameraRotationEulers.z));
                    break;
            }
        }


        protected virtual void ApplyCameraTransforms(float deltaTime)
        {
            CinemachineTransposer transposer = MainCamera.Instance.CameraTransposer;
            Transform cameraTransform = MainCamera.Instance.VirtualCamera.transform;

            if (transposer == null || cameraTransform == null)
                return;

            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, followSpeed * deltaTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }
}