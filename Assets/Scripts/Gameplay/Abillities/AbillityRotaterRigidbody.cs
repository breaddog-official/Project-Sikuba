using Mirror;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityRotaterRigidbody : AbillityRotater
    {
        [field: SerializeField] public float RotationSpeed { get; private set; } = 10;
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
            //tf.rotation = GetSmoothRotation(CalculateRotationToScreenPoint(point));
            Quaternion rotation = GetSmoothRotation(CalculateRotationToScreenPoint(point));

            /*Vector3 curForward = tf.forward;
            Vector3 targetForward = CalculateTargetToScreenPoint(point);

            Vector3 torque = Vector3.Cross(curForward, targetForward);

            rb.AddTorque(torque * RotationSpeed, ForceMode.Impulse);
            rb.AddTorque(-rb.angularVelocity);*/


            HandleRotationToPoint(rotation);

            if (!isServer) CmdHandleRotationToPoint(rotation);
        }

        public override void Rotate(Vector3 vector)
        {
            Quaternion rotation = GetSmoothRotation(Quaternion.LookRotation(vector));


            HandleRotation(rotation);

            if (!isServer) CmdHandleRotation(rotation);
        }

        #region Handlers
        [Command(channel = 2)]
        private void CmdHandleRotation(Quaternion rotation) => HandleRotation(rotation);
        private void HandleRotation(Quaternion rotation)
        {
            tf.rotation = rotation;
        }


        [Command(channel = 2)]
        private void CmdHandleRotationToPoint(Quaternion rotation) => HandleRotationToPoint(rotation);
        private void HandleRotationToPoint(Quaternion rotation)
        {
            rb.MoveRotation(rotation);
        }
        #endregion

        #region Calculations
        private Quaternion CalculateRotationToScreenPoint(Vector2 point)
        {
            Ray ray = MainCamera.Instance.Camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, MaxRayDistance, ~IgnoreLayers))
            {
                Vector3 currentLookVector = new Vector3(hitInfo.point.x, tf.position.y, hitInfo.point.z);
                Vector3 targetVector = currentLookVector - tf.position;

                Quaternion targetRotation = Quaternion.LookRotation(targetVector);

                return targetRotation;
            }

            return tf.rotation;
        }

        private Vector3 CalculateTargetToScreenPoint(Vector2 point)
        {
            Ray ray = MainCamera.Instance.Camera.ScreenPointToRay(point);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, MaxRayDistance, ~IgnoreLayers))
            {
                Vector3 currentLookVector = new Vector3(hitInfo.point.x, tf.position.y, hitInfo.point.z);
                Vector3 targetVector = currentLookVector - tf.position;

                return targetVector;
            }

            return tf.forward;
        }

        private Quaternion GetSmoothRotation(Quaternion quaternion)
        {
            return Quaternion.Slerp(tf.rotation, quaternion, RotationSpeed * Time.deltaTime);
        }
        #endregion

        public override bool IsPhysicsRotater() => true;
    }
}
