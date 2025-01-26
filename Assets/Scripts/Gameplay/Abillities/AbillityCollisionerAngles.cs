using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Scripts.Extensions;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCollisionerAngles : AbillityCollisioner
    {
        private enum UpMode
        {
            WorldUp,
            LocalUp
        }

        [SerializeField] private float maxGroundAngle;
        [SerializeField] private UpMode upMode;

        [field: ShowNonSerializedField]
        private bool onGround;
        private readonly HashSet<Collider> grounds = new(4);



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

                    SetOnGround(true);

                    return;
                }
                else
                {
                    RemoveCollision(collision);
                }
            }
        }

        private void RemoveCollision(Collision collision)
        {
            // We don't call Contains because Remove already checks for this
            grounds.Remove(collision.collider);

            if (grounds.Count == 0)
                SetOnGround(false);
        }



        public override bool InAir() => !onGround;

        public override bool OnGround() => onGround;


        private bool IsGround(Vector3 vector)
        {
            return Vector3.Angle(GetUp(), vector) <= maxGroundAngle;
        }


        private Vector3 GetUp()
        {
            return upMode switch
            {
                UpMode.WorldUp => Vector3.up,
                UpMode.LocalUp => transform.up,
                _ => throw new NotImplementedException()
            };
        }

        private void SetOnGround(bool value)
        {
            onGround = value;
            ChangeGroundedInvoke();
        }
    }
}