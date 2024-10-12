using Mirror;
using Scripts.Extensions;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityMoveRigidbody : AbillityMove
    {
        public enum AirMoveMode
        {
            Normal,
            Multiplied,
            None
        }

        [field: SerializeField] public float Speed { get; private set; } = 10.0f;
        [field: SerializeField] public float MinInput { get; private set; } = 0.01f;
        [field: Space]
        [field: SerializeField] public AirMoveMode AirMove { get; private set; }
        [field: SerializeField] public float AirMoveMultiplier { get; private set; } = 0.5f;

        private Rigidbody rb;
        private AbillityCollisioner collisioner;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            collisioner = GetComponent<AbillityCollisioner>();
        }



        public override void Move(Vector3 input)
        {
            if (input.sqrMagnitude < MinInput)
                return;

            Quaternion cameraRot = MainCamera.Instance.transform.rotation;

            // Apply local movement
            HandleMovement(input, cameraRot);
            
            // Apply movement on server
            if (!isServer) CmdHandleMovement(input, cameraRot);

            base.Move(input);
        }





        private void HandleMovement(Vector3 input, Quaternion cameraRotation)
        {
            // Skip move if moving in air disabled
            if (AirMove == AirMoveMode.None && collisioner.InAir())
                return;


            // Calculate move vector
            Vector3 calculatedVector = CalculateVector(input, cameraRotation);

            // We work only with 2 axis
            calculatedVector.y = 0.0f;


            // Apply multiply if needed
            if (AirMove == AirMoveMode.Multiplied && collisioner.InAir())
                calculatedVector *= AirMoveMultiplier;


            // Apply movement
            rb.AddForce(calculatedVector, ForceMode.Impulse);
        }

        [Command(channel = 2)]
        private void CmdHandleMovement(Vector3 input, Quaternion cameraRotation)
        {
            HandleMovement(input, cameraRotation);
        }

        private Vector3 CalculateVector(Vector3 input, Quaternion cameraRotation)
        {
            // We leave only rotation along the Y
            cameraRotation = Quaternion.Euler(0.0f, cameraRotation.eulerAngles.y, 0.0f);

            // Same as camera.TransformDirection(input)
            return cameraRotation * (input * Time.fixedDeltaTime) * (Speed * 10.0f);
        }




        /*private Vector3 lastInput;
        private Vector3 smoothInput;

        private Vector3 CalculateVector(Vector3 input, Quaternion cameraRotation)
        {
            // Calculate smooth input
            Vector3 smoothInput = GetSmoothInput(input.normalized);

            // We leave only rotation along the Y
            cameraRotation = Quaternion.Euler(0.0f, cameraRotation.eulerAngles.y, 0.0f);

            // Same as camera.TransformDirection(input)
            return cameraRotation * (smoothInput * Time.fixedDeltaTime) * (Speed * 10.0f);
        }

        private Vector3 GetSmoothInput(Vector3 currentInput)
        {
            lastInput = Vector3.SmoothDamp(lastInput, currentInput, ref smoothInput, MovementAcceleration);
            return lastInput;
        }*/


        public override bool IsPhysicsMovement() => true;
    }
}
