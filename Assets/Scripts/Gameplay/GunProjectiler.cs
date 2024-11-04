using Mirror;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectiler : Item
    {
        [field: SerializeField] public bool IsAutomaticFire { get; protected set; }
        [field: SerializeField] public bool IsReloadable { get; protected set; }
        [field: SerializeField] public uint MaxAmmo { get; protected set; }
        [Space]
        [SerializeField] protected Transform projectilePrefab;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected ParticleSystem shootParticle;

        public uint CurrentAmmo { get; protected set; }

        private Rigidbody rb;
        private Collider col;



        private void Awake()
        {
            col = GetComponent<Collider>();
            rb = col.attachedRigidbody;
        }


        public override void StartUsing()
        {
            print("start firing");
        }

        public override void StopUsing()
        {
            print("stop firing");
        }


        public override void OnEquip(NetworkConnectionToClient conn)
        {
            col.isTrigger = true;
            rb.useGravity = false;
        }
        public override void OnDequip()
        {
            col.isTrigger = false;
            rb.useGravity = true;
        }
    }
}
