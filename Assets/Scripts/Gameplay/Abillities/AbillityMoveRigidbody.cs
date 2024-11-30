using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;
using Scripts.MonoCache;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityMoveRigidbody : AbillityMove, IMonoCacheFixedUpdate
    {
        public enum AirMoveMode
        {
            Normal,
            Multiplied,
            None
        }

        [field: SerializeField] public int Channel { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 10.0f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 10.0f;
        [field: Space]
        [field: SerializeField] public BandwidthOptimizationMode OptimizationMode { get; private set; } = BandwidthOptimizationMode.Normal;
        [field: SerializeField] public float MinInput { get; private set; } = 0.01f;
        [field: Space]
        [field: SerializeField] public AirMoveMode AirMove { get; private set; }
        [field: SerializeField] public float AirMoveMultiplier { get; private set; } = 0.5f;

        private PredictedRigidbody rb;
        private AbillityCollisioner collisioner;

        public Behaviour Behaviour => this;


        public override void Initialize()
        {
            base.Initialize();

            MonoCacher.Registrate(this);

            rb = GetComponent<PredictedRigidbody>();
            collisioner = Entity.FindAbillity<AbillityCollisioner>();
        }

        public void FixedUpdateCached()
        {
            if (!isServer && !isOwned && !IsInitialized)
                return;

            if (rb.predictedRigidbody.velocity.magnitude > MaxSpeed)
                rb.predictedRigidbody.velocity = rb.predictedRigidbody.velocity.normalized * MaxSpeed;
        }


        public override void Move(Vector3 input)
        {
            // We don't calculate the input for reducing bandwidth if it is very small
            if (OptimizationMode != BandwidthOptimizationMode.None && input.Max() < MinInput)
                return;

            // Calculate camera relative input
            Vector3 relativeInput = MainCamera.Instance.transform.TransformDirection(input);

            // Apply local movement
            ApplyMovement(relativeInput);
            
            // Apply movement on server
            if (!isServer) ServerApplyMovement(relativeInput);


            base.Move(input);
        }





        private void ApplyMovement(Vector3 input)
        {
            // We don't calculate the input for increasing server's perfomance if it is very small

            // We will get a very small increase in performance and it will be more logical to calculate
            // the input anyway for greater prediction accuracy, so this optimization is classified as Aggressive.
            if (OptimizationMode == BandwidthOptimizationMode.Aggressive && isServer && input.Max() < MinInput)
                return;

            // Skip move if moving in air disabled
            if (AirMove == AirMoveMode.None && collisioner.InAir())
                return;


            // Calculate move vector
            Vector3 calculatedVector = input.normalized * (Time.fixedDeltaTime * Speed * 10.0f);

            // We work only with 2 axis
            calculatedVector.y = 0.0f;

            // Apply multiply if needed
            if (AirMove == AirMoveMode.Multiplied && collisioner.InAir())
                calculatedVector *= AirMoveMultiplier;



            // Apply movement
            rb.predictedRigidbody.AddForce(calculatedVector, ForceMode.Impulse);
        }

        [Client]
        protected virtual void ServerApplyMovement(Vector3 input)
        {
            switch (Channel)
            {
                case Channels.Reliable:
                    CmdApplyMovementReliable(input);
                    break;

                default:
                case Channels.Unreliable:
                    CmdApplyMovementUnreliable(input);
                    break;
            }
        }

        [Command(channel = 0)]
        private void CmdApplyMovementReliable(Vector3 input) => ApplyMovement(input);

        // Channel 2 is usually UnrealiableSequenced, instead simple Unrealiable
        [Command(channel = 2)]
        private void CmdApplyMovementUnreliable(Vector3 input) => ApplyMovement(input);



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
