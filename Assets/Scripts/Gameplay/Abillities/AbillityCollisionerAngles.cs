using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCollisionerAngles : AbillityCollisioner
    {
        [SerializeField] private float maxGroundAngle;

        //[ShowNonSerializedField]
        private bool onGround;
        private HashSet<Collider> grounds = new(4);


        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (grounds.Contains(collision.collider))
            {
                grounds.Remove(collision.collider);

                if (grounds.Count == 0)
                    onGround = false;
            }
        }




        private void HandleCollision(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsGround(contact.normal) && !grounds.Contains(contact.otherCollider))
                {
                    onGround = true;
                    grounds.Add(contact.otherCollider);
                    return;
                }
                else if (grounds.Contains(contact.otherCollider))
                {
                    grounds.Remove(contact.otherCollider);

                    if (grounds.Count == 0)
                        onGround = false;
                }
            }
        }



        public override bool InAir() => !onGround;

        public override bool OnGround() => onGround;


        private bool IsGround(Vector3 vector)
        {
            return Vector3.Angle(Vector3.up, vector) <= maxGroundAngle;
        }
    }
}