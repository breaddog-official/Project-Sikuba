using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityJumpRigidbody : AbillityJump
    {
        [field: SerializeField] public float JumpForce { get; private set; } = 25.0f;
        [field: SerializeField] public float MaxGroundAngle { get; private set; } = 25.0f;

        [field: ShowNonSerializedField]
        public bool IsGround { get; private set; } = false;

        private Rigidbody rb;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        public override bool TryJump()
        {
            if (!CanJump())
                return false;


            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);


            return base.TryJump();
        }


        public override bool CanJump()
        {
            return IsGround;
        }



        private void OnCollisionStay(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (IsGroundContact(collision.GetContact(i)))
                {
                    IsGround = true;
                    break;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (IsGroundContact(collision.GetContact(i)))
                {
                    IsGround = false;
                    break;
                }
            }
        }



        private bool IsGroundContact(ContactPoint contact)
            => Vector3.Angle(contact.normal, transform.up) <= MaxGroundAngle;
    }
}
