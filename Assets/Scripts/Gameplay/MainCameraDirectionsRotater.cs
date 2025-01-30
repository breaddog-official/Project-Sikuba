using NaughtyAttributes;
using Scripts.MonoCache;
using UnityEngine;
using Scripts.Extensions;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Scripts.Input;
using TMPro;

namespace Scripts.Gameplay.CameraManagement
{
    public class MainCameraDirectionsRotater : MainCamera, IMonoCacheUpdate
    {
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

        bool isSubscribed;


        private void Start()
        {
            targetOffset = CinemachineFollow.FollowOffset;
            targetRotation = CinemachineCamera.transform.rotation;

            currentDirection = initialDirection;

            MonoCacher.Registrate(this);

            Subscribe();
            ApplyDirection();
        }

        public void UpdateCached()
        {
            ApplyCameraTransforms(Time.deltaTime);
        }


        protected virtual void OnEnable() => Subscribe();
        protected virtual void OnDisable() => Unsubscribe();


        private void Subscribe()
        {
            if (isSubscribed.CheckInitialization())
                return;

            InputManager.Controls.Game.Turn.performed += CameraTurnAction;
        }

        private void Unsubscribe()
        {
            InputManager.Controls.Game.Turn.performed -= CameraTurnAction;
        }



        public virtual void TurnRight()
        {
            currentDirection.DecreaseInBounds(directionsCount, true);

            ApplyDirection();
        }

        public virtual void TurnLeft()
        {
            currentDirection.IncreaseInBounds(directionsCount, false);

            ApplyDirection();
        }



        protected virtual void ApplyDirection()
        {
            Vector3 cameraRotationEulers = transform.rotation.eulerAngles;


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
            CinemachineFollow transposer = CinemachineFollow;
            Transform cameraTransform = CinemachineCamera.transform;

            if (transposer == null || cameraTransform == null)
                return;

            transposer.FollowOffset = Vector3.Lerp(transposer.FollowOffset, targetOffset, followSpeed * deltaTime);
            //cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotateSpeed * deltaTime);

            if (transposer.FollowTarget)
            {
                var lookPos = transposer.FollowTarget.position - cameraTransform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, rotation, rotateSpeed * deltaTime);
            }
        }



        private void CameraTurnAction(InputAction.CallbackContext ctx = default)
        {
            float value = ctx.action.ReadValue<float>();

            if (value < 0)
                TurnLeft();

            else if (value > 0)
                TurnRight();
        }
    }
}