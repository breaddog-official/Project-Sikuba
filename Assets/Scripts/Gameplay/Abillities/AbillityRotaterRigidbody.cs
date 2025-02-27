﻿using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.Gameplay.CameraManagement;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityRotaterRigidbody : AbillityRotater
    {
        [field: SerializeField] public Channels Channel { get; private set; }
        [field: Space]
        [field: SerializeField] public float RotationSpeed { get; private set; } = 10;
        [field: Space]
        [field: SerializeField] public BandwidthOptimizationMode OptimizationMode { get; private set; } = BandwidthOptimizationMode.Normal;
        [field: HideIf(nameof(OptimizationMode), BandwidthOptimizationMode.None)]
        [field: SerializeField] public float MinInput { get; private set; } = 0.01f;
        [field: Space]
        [field: SerializeField] public float MaxRayDistance { get; private set; } = 300;
        [field: SerializeField] public LayerMask IgnoreLayers { get; private set; }

        private Rigidbody rb;
        private Transform tf;


        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            rb = GetComponent<Rigidbody>();
            tf = transform;

            return true;
        }



        public override void RotateToPoint(Vector2 point)
        {
            Quaternion rotation = CalculateRotationToScreenPoint(point);
            Vector3 rotationEulers = rotation.eulerAngles;

            //Vector3 curForward = tf.forward;
            //Vector3 targetForward = CalculateVectorToScreenPoint(point);

            //Vector3 rotationEulers = GetSmoothVector(Vector3.Cross(curForward, targetForward));




            ApplyRotation(rotationEulers);

            if (!isServer) ServerApplyRotation(rotationEulers);
        }

        public override void Rotate(Vector3 input)
        {
            // We don't calculate the input for reducing bandwidth if it is very small
            if (OptimizationMode != BandwidthOptimizationMode.None && input.Max() < MinInput)
                return;

            Quaternion rotation = Quaternion.LookRotation(input);


            ApplyRotation(rotation.eulerAngles);

            if (!isServer) ServerApplyRotation(rotation.eulerAngles);
        }

        #region Applying

        [Client]
        protected virtual void ServerApplyRotation(Vector3 rotationEulers)
        {
            switch (Channel)
            {
                default:
                case Channels.Reliable:
                    CmdApplyRotationReliable(rotationEulers);
                    break;

                case Channels.Unreliable:
                    CmdApplyRotationUnreliable(rotationEulers);
                    break;

                case Channels.UnreliableSequenced:
                    CmdApplyRotationUnreliableSequenced(rotationEulers);
                    break;
            }
        }

        [Command(channel = 0)]
        private void CmdApplyRotationReliable(Vector3 rotationEulers) => ApplyRotation(rotationEulers);
        
        [Command(channel = 1)]
        private void CmdApplyRotationUnreliable(Vector3 rotationEulers) => ApplyRotation(rotationEulers);

        [Command(channel = 2)]
        private void CmdApplyRotationUnreliableSequenced(Vector3 rotationEulers) => ApplyRotation(rotationEulers);


        private void ApplyRotation(Vector3 rotationEulers)
        {
            // To reduce bandwidth, we use euler angles because they contain 3 floats unlike quaternions which contain 4
            Quaternion rotation = Quaternion.Euler(rotationEulers);

            // Apply smooth rotation
            rotation = GetSmoothRotation(rotation);



            rb.MoveRotation(rotation);

            //rb.AddTorque(-rb.angularVelocity);
            //rb.AddTorque(rotationEulers * RotationSpeed, ForceMode.Impulse);

        }
        #endregion

        #region Calculations
        private Quaternion CalculateRotationToScreenPoint(Vector2 point)
        {
            Vector3 cameraPoint = ThrowCameraRay(point);
            Vector3 targetVector = CalculateVectorToScreenPoint(point, cameraPoint);
            Quaternion targetRotation = Quaternion.LookRotation(targetVector);

            return targetRotation;
        }

        private Vector3 CalculateVectorToScreenPoint(Vector2 point, Vector3? screenPointInternal = null)
        {
            // Creates non ? struct
            Vector3 screenPoint;

            if (screenPointInternal.HasValue)
                screenPoint = screenPointInternal.Value;

            else
                screenPoint = ThrowCameraRay(point);

            // Otherwise - calculating
            Vector3 currentLookVector = new Vector3(screenPoint.x, tf.position.y, screenPoint.z);
            Vector3 targetVector = currentLookVector - tf.position;

            return targetVector;
        }

        private Vector3 ThrowCameraRay(Vector2 point)
        {
            Ray ray = MainCamera.Instance.Camera.ScreenPointToRay(point);
            Plane plane = new Plane(transform.up, transform.position);
            plane.Raycast(ray, out float enter);

            return ray.GetPoint(enter);
        }

        private Quaternion GetSmoothRotation(Quaternion quaternion)
        {
            return Quaternion.Lerp(tf.rotation, quaternion, RotationSpeed * GetDeltaTime());
        }

        private Vector3 GetSmoothVector(Vector3 rotation)
        {
            return Vector3.Lerp(tf.rotation.eulerAngles, rotation, RotationSpeed * GetDeltaTime());
        }

        private float GetDeltaTime()
        {
            return IsPhysicsRotater() ? Time.fixedDeltaTime : Time.deltaTime;
        }

        #endregion

        public override bool IsPhysicsRotater() => true;
    }
}
