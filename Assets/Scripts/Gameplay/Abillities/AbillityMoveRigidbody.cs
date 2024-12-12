using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.Gameplay.Entities;
using Scripts.MonoCache;
using Scripts.Network;
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

        [field: Dropdown(nameof(GetChannelValues))]
        [field: SerializeField] public int Channel { get; private set; }
        [field: Space]
        [field: SerializeField] public float Speed { get; private set; } = 10.0f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 10.0f;
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
            base.Initialize();

            MonoCacher.Registrate(this);

            rb = GetComponent<PredictedRigidbody>();
            collisioner = Entity.FindAbillity<AbillityCollisioner>();

            return true;
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
            Vector3 calculatedVector = input.normalized * (GetDeltaTime() * Speed * 10.0f);

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
                case 2:
                    CmdApplyMovementUnreliable(input);
                    break;
            }
        }

        [Command(channel = 0)]
        private void CmdApplyMovementReliable(Vector3 input) => ApplyMovement(input);

        // Channel 2 is usually UnrealiableSequenced, instead simple Unrealiable
        [Command(channel = 2)]
        private void CmdApplyMovementUnreliable(Vector3 input) => ApplyMovement(input);



        private float GetDeltaTime()
        {
            return IsPhysicsMovement() ? Time.fixedDeltaTime : Time.deltaTime;
        }



        protected virtual DropdownList<int> GetChannelValues()
        {
            return new()
            {
                { "Reliable",   Mirror.Channels.Reliable },
                { "Unreilable",   Mirror.Channels.Unreliable },
                { "Unreliable Sequenced",    2 },
            };
        }

        public override bool IsPhysicsMovement() => true;
    }
}
