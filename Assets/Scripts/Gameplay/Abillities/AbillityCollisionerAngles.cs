using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCollisionerAngles : AbillityCollisioner
    {
        [SerializeField] private float maxGroundAngle;

        //[ShowNonSerializedField]
        private bool onGround;
        private HashSet<Collider> grounds;


        private void Awake()
        {
            grounds = new(4);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            RemoveCollision(collision);
        }




        private void HandleCollision(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // We don't call Contains because Add and Remove already checks for this

                if (IsGround(contact.normal))
                {
                    grounds.Add(contact.otherCollider);

                    onGround = true;

                    return;
                }
                else
                {
                    grounds.Remove(contact.otherCollider);

                    if (grounds.Count == 0)
                        onGround = false;
                }
            }
        }

        private void RemoveCollision(Collision collision)
        {
            // We don't call Contains because Remove already checks for this
            grounds.Remove(collision.collider);

            if (grounds.Count == 0)
                onGround = false;
        }



        public override bool InAir() => !onGround;

        public override bool OnGround() => onGround;


        private bool IsGround(Vector3 vector)
        {
            return Vector3.Angle(Vector3.up, vector) <= maxGroundAngle;
        }
    }
}