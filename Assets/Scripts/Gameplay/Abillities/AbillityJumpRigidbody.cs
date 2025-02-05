using Mirror;
using Scripts.Gameplay.Entities;
using Scripts.MonoCache;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityJumpRigidbody : AbillityJump, IMonoCacheFixedUpdate
    {
        [field: SerializeField] public float JumpForce { get; private set; } = 25.0f;
        [field: SerializeField] public bool DisableDragInAir { get; private set; }
        [field: SerializeField] public ForceMode ForceMode { get; private set; } = ForceMode.VelocityChange;

        private PredictedRigidbody rb;
        private AbillityCollisioner collisioner;

        public Behaviour Behaviour => this;

        private float? lastDrag;



        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            rb = GetComponent<PredictedRigidbody>();
            collisioner = GetComponent<Entity>().FindAbillity<AbillityCollisioner>();

            MonoCacher.Registrate(this);

            return true;
        }


        public void FixedUpdateCached()
        {
            if (!DisableDragInAir)
                return;

            if (collisioner.InAir())
            {
                lastDrag ??= rb.predictedRigidbody.linearDamping;
                rb.predictedRigidbody.linearDamping = 0.0f;
            }
            else
            {
                rb.predictedRigidbody.linearDamping = lastDrag ?? rb.predictedRigidbody.linearDamping;
                lastDrag = null;
            }
        }


        public override void Jump()
        {
            if (!CanJump())
                return;

            // Apply jump locally
            ApplyJump();

            // Send jump to server
            if (!isServer) CmdJump();


            base.Jump();
        }

        [Command]
        private void CmdJump()
        {
            if (CanJump())
                ApplyJump();
        }

        private void ApplyJump()
        {
            rb.predictedRigidbody.AddForce(Vector3.up * JumpForce, ForceMode);
        }


        public bool CanJump()
            => collisioner.OnGround();
    }
}
