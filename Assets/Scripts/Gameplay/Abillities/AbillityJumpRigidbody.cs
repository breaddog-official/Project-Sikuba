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

        private PredictedRigidbody rb;
        private AbillityCollisioner collisioner;

        public Behaviour Behaviour => this;

        private float? lastDrag;



        public override void Initialize()
        {
            rb = GetComponent<PredictedRigidbody>();
            collisioner = GetComponent<Entity>().FindAbillity<AbillityCollisioner>();

            MonoCacher.Registrate(this);
        }


        public void FixedUpdateCached()
        {
            if (!DisableDragInAir)
                return;

            if (collisioner.InAir())
            {
                lastDrag ??= rb.predictedRigidbody.drag;
                rb.predictedRigidbody.drag = 0.0f;
            }
            else
            {
                rb.predictedRigidbody.drag = lastDrag ?? rb.predictedRigidbody.drag;
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
            rb.predictedRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }


        public bool CanJump()
            => collisioner.OnGround();
    }
}
