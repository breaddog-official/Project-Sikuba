using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityMoveRigidbody : AbillityMove
    {
        [field: SerializeField] public float Speed { get; private set; } = 10.0f;

        private Rigidbody rb;




        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }




        public override void Move(Vector3 input)
        {
            Quaternion cameraRot = MainCamera.Instance.transform.rotation;

            HandleMovement(input, cameraRot);

            base.Move(input);
        }




        private void AddForce(Vector3 force)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }

        private void HandleMovement(Vector3 input, Quaternion cameraRotation)
        {
            Vector3 calculatedVector = CalculateVector(input, cameraRotation);
            AddForce(calculatedVector);
        }



        private Vector3 CalculateVector(Vector3 input, Quaternion cameraRotation)
        {
            // We leave only rotation along the Y
            cameraRotation = Quaternion.Euler(0.0f, cameraRotation.eulerAngles.y, 0.0f);

            // Same as camera.TransformDirection(input)
            return cameraRotation * (input.normalized * Time.deltaTime) * (Speed * 10.0f);
        }



        // Although this component is powered by physics, to avoid input loss we will process it in Update
        public override bool IsPhysicsMovement() => false;
    }
}
