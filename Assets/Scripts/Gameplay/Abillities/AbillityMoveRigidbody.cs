using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.CameraManagement;
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

        [field: SerializeField] public Channels Channel { get; private set; }
        [field: Space]
        [field: SerializeField] public float Acceleration { get; private set; } = 10.0f;
        [field: SerializeField] public float Speed { get; private set; } = 10.0f;
        [field: SerializeField] public GenVector3<bool> MoveAxis { get; private set; } = new(true, false, true);
        [field: Space]
        [field: SerializeField] public BandwidthOptimizationMode OptimizationMode { get; private set; } = BandwidthOptimizationMode.Normal;
        [field: HideIf(nameof(OptimizationMode), BandwidthOptimizationMode.None)]
        [field: SerializeField] public float MinInput { get; private set; } = 0.01f;
        [field: Space]
        [field: SerializeField] public AirMoveMode AirMove { get; private set; }
        [field: ShowIf(nameof(AirMove), AirMoveMode.Multiplied)]
        [field: SerializeField] public float AirMoveMultiplier { get; private set; } = 0.5f;

        private PredictedRigidbody rb;
        private AbillityCollisioner collisioner;

        public Behaviour Behaviour => this;


        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            MonoCacher.Registrate(this);

            rb = GetComponent<PredictedRigidbody>();
            collisioner = Entity.FindAbillity<AbillityCollisioner>();

            return true;
        }

        public void FixedUpdateCached()
        {
            if (!isServer && !isOwned && !IsInitialized)
                return;

            if (rb.predictedRigidbody.linearVelocity.magnitude > Speed)
            {
                var fixedVector = rb.predictedRigidbody.linearVelocity.normalized * Speed;
                rb.predictedRigidbody.linearVelocity = new Vector3(MoveAxis.x ? fixedVector.x : rb.predictedRigidbody.linearVelocity.x,
                                                                   MoveAxis.y ? fixedVector.y : rb.predictedRigidbody.linearVelocity.y,
                                                                   MoveAxis.z ? fixedVector.z : rb.predictedRigidbody.linearVelocity.z);
            }
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


            base.Move(relativeInput);
        }





        private void ApplyMovement(Vector3 input)
        {
            // We don't calculate the input for increasing server's perfomance if it is very small

            // We will get a very small increase in performance and it will be more logical to calculate
            // the input anyway for greater prediction accuracy, so this optimization is classified as Aggressive.
            if (OptimizationMode == BandwidthOptimizationMode.Aggressive && NetworkServer.active && input.Max() < MinInput)
                return;

            // Skip move if moving in air disabled
            if (AirMove == AirMoveMode.None && collisioner.InAir())
                return;


            // Calculate move vector
            Vector3 calculatedVector = input.normalized * (GetDeltaTime() * Acceleration * 10.0f);

            // Ignore disabled axis
            calculatedVector = Vector3.Scale(calculatedVector, MoveAxis.ToInteger());

            // Apply multiply if needed
            if (AirMove == AirMoveMode.Multiplied && collisioner.InAir())
                calculatedVector *= AirMoveMultiplier;



            // Apply movement
            rb.predictedRigidbody.AddForce(calculatedVector, ForceMode.VelocityChange);
        }

        [Client]
        protected virtual void ServerApplyMovement(Vector3 input)
        {
            switch (Channel)
            {
                default:
                case Channels.Reliable:
                    CmdApplyMovementReliable(input);
                    break;

                case Channels.Unreliable:
                    CmdApplyMovementUnreliable(input);
                    break;

                case Channels.UnreliableSequenced:
                    CmdApplyMovementUnreliableSequenced(input);
                    break;
            }
        }

        [Command(channel = 0)]
        private void CmdApplyMovementReliable(Vector3 input) => ApplyMovement(input);

        [Command(channel = 1)]
        private void CmdApplyMovementUnreliable(Vector3 input) => ApplyMovement(input);

        [Command(channel = 2)]
        private void CmdApplyMovementUnreliableSequenced(Vector3 input) => ApplyMovement(input);



        private float GetDeltaTime()
        {
            return IsPhysicsMovement() ? Time.fixedDeltaTime : Time.deltaTime;
        }



        public override bool IsPhysicsMovement() => true;
    }
}
