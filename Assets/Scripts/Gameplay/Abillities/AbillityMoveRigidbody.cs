using Mirror;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityMoveRigidbody : AbillityMove
    {
        [field: SerializeField] public float Speed { get; private set; } 

        private Rigidbody rb;




        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }




        public override void Move(Vector3 vector)
        {
            vector = transform.TransformDirection(vector.normalized) * Speed;

            AddForce(vector);

            if (NetworkClient.active)
                CmdAddForce(vector);

            base.Move(vector);
        }



        [Command]
        private void CmdAddForce(Vector3 force) => AddForce(force);
        private void AddForce(Vector3 force) => rb.AddForce(force, ForceMode.Force);


        public override bool IsPhysicsMovement() => true;
    }
}
