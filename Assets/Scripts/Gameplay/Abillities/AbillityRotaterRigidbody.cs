using System.Drawing;
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
            if (point.sqrMagnitude < MinInput)
                return;

            //tf.rotation = GetSmoothRotation(CalculateRotationToScreenPoint(point));
            //rb.MoveRotation(GetSmoothRotation(CalculateRotationToScreenPoint(point)));

            Vector3 curForward = tf.forward;
            Vector3 targetForward = CalculateTargetToScreenPoint(point);

            Vector3 torque = Vector3.Cross(curForward, targetForward);

            rb.AddTorque(torque * RotationSpeed, ForceMode.Impulse);
            rb.AddTorque(-rb.angularVelocity);
        }

        public override void Rotate(Vector3 vector)
        {
            if (vector.sqrMagnitude < MinInput)
                return;

            tf.rotation = GetSmoothRotation(Quaternion.LookRotation(vector));
        }

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


        public override bool IsPhysicsRotater() => true;
    }
}
