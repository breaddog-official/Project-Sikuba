using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Gameplay.Entities;
using System.Threading;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectiler : Item
    {
        [SerializeField] public bool isAutomaticFire;
        [SerializeField] public bool isReloadable;
        [SerializeField] public uint maxAmmo;
        [Space]
        [SerializeField] public int cooldown;
        [SerializeField] public float forceAmount;
        [SerializeField] public float torqueAmount;
        [Space]
        [SerializeField] protected Projectile projectilePrefab;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected Effector shootEffector;

        public uint CurrentAmmo { get; protected set; }

        public bool IsEnoughAmmo => isReloadable ? CurrentAmmo > 0 : true;

        private Rigidbody rb;
        private Collider col;

        private CancellationTokenSource cancellationToken;

        private bool canShoot = true;


        private void Awake()
        {
            col = GetComponent<Collider>();
            rb = col.attachedRigidbody;

            if (!isOwned && Owner != null)
            {
                col.isTrigger = true;
            }
        }

        [Command]
        public override void StartUsing()
        {
            FireLoop().Forget();

            print("start firing");
        }

        [Command]
        public override void StopUsing()
        {
            cancellationToken?.Cancel();

            print("stop firing");
        }


        [Server]
        public override void OnEquip()
        {
            col.isTrigger = true;
            rb.useGravity = false;
        }
        [Server]
        public override void OnDequip()
        {
            col.isTrigger = false;
            rb.useGravity = true;

            cancellationToken?.Cancel();
        }


        [Server]
        protected virtual void Fire()
        {
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

            NetworkServer.Spawn(projectile.gameObject);

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.AddForce(projectile.transform.forward * forceAmount);
            projectileRb.AddTorque(projectile.transform.forward * torqueAmount);

            projectile.Initialize(Owner);

            if (shootEffector != null)
                shootEffector.Play();

            CurrentAmmo--;
        }

        protected async UniTaskVoid FireLoop()
        {
            cancellationToken?.Cancel();
            cancellationToken?.Dispose();
            cancellationToken = new();

            while (IsEnoughAmmo)
            {
                if (!CanShoot())
                    await UniTask.WaitUntil(CanShoot, cancellationToken: cancellationToken.Token);

                Fire();

                WaitCooldown().Forget();

                if (!isAutomaticFire)
                    break;
            }
        }


        protected async UniTaskVoid WaitCooldown()
        {
            canShoot = false;

            await UniTask.Delay(cooldown);

            canShoot = true;
        }

        private void OnDestroy()
        {
            cancellationToken?.Cancel();
        }


        protected bool CanShoot() => canShoot;
    }
}
