using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCollisionerAngles : AbillityCollisioner
    {
        [SerializeField] private float maxGroundAngle;

        //[ShowNonSerializedField]
        [SerializeField]
        private bool onGround;
        private GameObject ground;




        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == ground)
            {
                onGround = false;
                ground = null;
            }
        }




        private void HandleCollision(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsGround(contact.normal))
                {
                    onGround = true;
                    ground = contact.otherCollider.gameObject;
                    return;
                }
                else if (contact.otherCollider.gameObject == ground)
                {
                    onGround = false;
                    ground = null;
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