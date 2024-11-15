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
            foreach (Collider collider in grounds)
            {
                print($"{collider}        {grounds.Count}       stay");
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            RemoveCollision(collision);
            foreach (Collider collider in grounds)
            {
                print($"{collider}        {grounds.Count}       exit");
            }
        }




        private void HandleCollision(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                bool containsCollider = grounds.Contains(contact.otherCollider);

                if (IsGround(contact.normal))
                {
                    onGround = true;

                    if(!containsCollider)
                        grounds.Add(contact.otherCollider);

                    return;
                }
                else if (containsCollider)
                {
                    grounds.Remove(contact.otherCollider);

                    if (grounds.Count == 0)
                        onGround = false;
                }
            }
        }

        private void RemoveCollision(Collision collision)
        {
            IEnumerable<Collider> colliders = collision.contacts.Select(p => p.otherCollider);

            grounds.RemoveWhere(g => colliders.Contains(g));

            /*IEnumerable<Collider> intersect = grounds.Intersect(collision.contacts
                                                                        .Select(p => p.otherCollider));
            foreach (var i in intersect) grounds.Remove(i);*/

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