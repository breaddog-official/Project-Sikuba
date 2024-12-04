using System;
using Cinemachine;
using Mirror;
using Scripts.MonoCache;
using UnityEditor;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCamera : Abillity, IMonoCacheUpdate
    {
        [SerializeField] private Transform target;
        [Space]
        [SerializeField] private Direction initialDirection;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float offsetYRotation;
        [Space]
        [SerializeField] private float followSpeed = 1f;
        [SerializeField] private float rotateSpeed = 1f;

        protected Direction currentDirection;

        Vector3 targetOffset;
        Quaternion targetRotation;

        public Behaviour Behaviour => this;


        protected enum Direction
        {
            Forward,
            Right,
            Backward,
            Left
        }

        protected int directionCount;


        public override void Initialize()
        {
            base.Initialize();

            targetOffset = MainCamera.Instance.CameraTransposer.m_FollowOffset;
            targetRotation = MainCamera.Instance.VirtualCamera.transform.rotation;

            MonoCacher.Registrate(this);

            SetFollow(true);
            SetDirection(initialDirection);

            directionCount = Enum.GetValues(typeof(Direction)).Length;
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
            Direction dir;

            if ((int)currentDirection == directionCount)
                dir = 0;
            else
                dir = currentDirection + 1;

            SetDirection(dir);
        }

        [Client]
        public virtual void TurnLeft()
        {
            Direction dir;

            if (currentDirection == 0)
                dir = (Direction)directionCount - 1;
            else
                dir = currentDirection - 1;

            SetDirection(dir);
        }



        protected virtual void SetDirection(Direction newDirection)
        {
            CinemachineTransposer transposer = MainCamera.Instance.CameraTransposer;
            Transform cameraTransform = MainCamera.Instance.VirtualCamera.transform;
            Vector3 cameraRotationEulers = cameraTransform.rotation.eulerAngles;

            if (transposer == null || cameraTransform == null)
                return;

            currentDirection = newDirection;

            switch (newDirection)
            {
                case Direction.Forward:
                    targetOffset = offset;
                    targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, offsetYRotation, cameraRotationEulers.z));
                    break;

                case Direction.Backward:
                    targetOffset = new Vector3(-offset.x, offset.y, -offset.z);
                    targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, -(90 + offsetYRotation), cameraRotationEulers.z));
                    break;

                case Direction.Right:
                    targetOffset = new Vector3(-offset.x, offset.y, offset.z);
                    targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, -offsetYRotation, cameraRotationEulers.z));
                    break;

                case Direction.Left:
                    targetOffset = new Vector3(offset.x, offset.y, -offset.z);
                    targetRotation = Quaternion.Euler(new Vector3(cameraRotationEulers.x, 90 + offsetYRotation, cameraRotationEulers.z));
                    break;
            }
        }


        protected virtual void ApplyCameraTransforms(float deltaTime)
        {
            CinemachineTransposer transposer = MainCamera.Instance.CameraTransposer;
            Transform cameraTransform = MainCamera.Instance.VirtualCamera.transform;

            if (transposer == null || cameraTransform == null)
                return;

            print(transposer.m_FollowOffset);
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, followSpeed * deltaTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }
}