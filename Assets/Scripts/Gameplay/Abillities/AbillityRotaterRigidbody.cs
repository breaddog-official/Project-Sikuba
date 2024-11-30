using Mirror;
using Scripts.Extensions;
using System.Drawing;
using UnityEngine;
using UnityEngine.Windows;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityRotaterRigidbody : AbillityRotater
    {
        [field: SerializeField] public int Channel { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; } = 10;
        [field: Space]
        [field: SerializeField] public BandwidthOptimizationMode OptimizationMode { get; private set; } = BandwidthOptimizationMode.Normal;
        [field: SerializeField] public float MinInput { get; private set; } = 0.01f;
        [field: Space]
        [field: SerializeField] public float MaxRayDistance { get; private set; } = 300;
        [field: SerializeField] public LayerMask IgnoreLayers { get; private set; }

        private Rigidbody rb;
        private Transform tf;


        public override void Initialize()
        {
            rb = GetComponent<Rigidbody>();
            tf = transform;

            base.Initialize();
        }



        public override void RotateToPoint(Vector2 point)
        {
            //Quaternion rotation = GetSmoothRotation(CalculateRotationToScreenPoint(point));

            Vector3 curForward = tf.forward;
            Vector3 targetForward = CalculateVectorToScreenPoint(point);

            Vector3 rotationEulers = GetSmoothVector(Vector3.Cross(curForward, targetForward));




            ApplyRotation(rotationEulers);

            if (!isServer) ServerApplyRotation(rotationEulers);
        }

        public override void Rotate(Vector3 vector)
        {
            // We don't calculate the input for reducing bandwidth if it is very small
            if (OptimizationMode != BandwidthOptimizationMode.None && vector.Max() < MinInput)
                return;

            Quaternion rotation = GetSmoothRotation(Quaternion.LookRotation(vector));


            ApplyRotation(rotation.eulerAngles);

            if (!isServer) ServerApplyRotation(rotation.eulerAngles);
        }

        #region Applying

        [Client]
        protected virtual void ServerApplyRotation(Vector3 rotationEulers)
        {
            switch (Channel)
            {
                case Channels.Reliable:
                    CmdApplyRotationReliable(rotationEulers);
                    break;

                default:
                case Channels.Unreliable:
                    CmdApplyRotationUnreliable(rotationEulers);
                    break;
            }
        }

        [Command(channel = 0)]
        private void CmdApplyRotationReliable(Vector3 rotationEulers) => ApplyRotation(rotationEulers);

        // Channel 2 is usually UnreliableSequenced, instead simple Unreliable
        [Command(channel = 2)]
        private void CmdApplyRotationUnreliable(Vector3 rotationEulers) => ApplyRotation(rotationEulers);


        private void ApplyRotation(Vector3 rotationEulers)
        {
            // To reduce bandwidth, we use euler angles because they contain 3 floats unlike quaternions which contain 4
            //Quaternion rotation = Quaternion.Euler(rotationEulers);

            //rb.MoveRotation(rotation);

            print(rotationEulers * RotationSpeed);
            rb.AddTorque(-rb.angularVelocity);
            rb.AddTorque(rotationEulers * RotationSpeed, ForceMode.Impulse);

        }
        #endregion

        #region Calculations
        private Quaternion CalculateRotationToScreenPoint(Vector2 point)
        {
            if (ThrowCameraRay(out RaycastHit hit, point))
            {
                Vector3 targetVector = CalculateVectorToScreenPoint(point, hit);
                Quaternion targetRotation = Quaternion.LookRotation(targetVector);

                return targetRotation;
            }

            return tf.rotation;
        }

        private Vector3 CalculateVectorToScreenPoint(Vector2 point, RaycastHit? hit = null)
        {
            // Creates non ? struct
            RaycastHit hitInfo = hit ?? hit.Value;

            // If we don't have hit and we can't throw our ray, return
            if (hit.HasValue == false && ThrowCameraRay(out hitInfo, point) == false)
                return tf.forward;

            // Otherwise - calculating
            Vector3 currentLookVector = new Vector3(hitInfo.point.x, tf.position.y, hitInfo.point.z);
            Vector3 targetVector = currentLookVector - tf.position;

            return targetVector;
        }

        private bool ThrowCameraRay(out RaycastHit hitInfo, Vector2 point)
        {
            Ray ray = MainCamera.Instance.Camera.ScreenPointToRay(point);

            return Physics.Raycast(ray, out hitInfo, MaxRayDistance, ~IgnoreLayers);
        }

        private Quaternion GetSmoothRotation(Quaternion quaternion)
        {
            return Quaternion.Slerp(tf.rotation, quaternion, RotationSpeed * GetDeltaTime());
        }

        private Vector3 GetSmoothVector(Vector3 rotation)
        {
            return Vector3.Slerp(tf.rotation.eulerAngles, rotation, RotationSpeed * GetDeltaTime());
        }

        private float GetDeltaTime()
        {
            return IsPhysicsRotater() ? Time.fixedDeltaTime : Time.deltaTime;
        }
        #endregion

        public override bool IsPhysicsRotater() => true;
    }
}
