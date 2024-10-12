using Mirror;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityJumpRigidbody : AbillityJump
    {
        [field: SerializeField] public float JumpForce { get; private set; } = 25.0f;
        [field: SerializeField] public bool DisableDragInAir { get; private set; }

        private Rigidbody rb;
        private AbillityCollisioner collisioner;

        private float? lastDrag;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            collisioner = GetComponent<AbillityCollisioner>();
        }


        private void FixedUpdate()
        {
            if (!DisableDragInAir)
                return;

            if (collisioner.InAir())
            {
                lastDrag ??= rb.drag;
                rb.drag = 0.0f;
            }
            else
            {
                rb.drag = lastDrag ?? rb.drag;
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
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }


        public bool CanJump()
            => collisioner.OnGround();
    }
}
