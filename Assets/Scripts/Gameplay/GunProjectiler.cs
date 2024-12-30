using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.SessionManagers;
using Scripts.Extensions;
using System.Threading;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class GunProjectiler : Item
    {
        [SerializeField] protected bool disableOnDequip;
        [Space]
        [SerializeField] protected bool isAutomaticFire;
        [SerializeField] protected bool isReloadable;
        [SerializeField] protected uint maxAmmo;
        [Space]
        [SerializeField] protected int cooldown;
        [SerializeField] protected float forceAmount;
        [SerializeField] protected float torqueAmount;
        [Space]
        [SerializeField] protected Projectile projectilePrefab;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected Effector shootEffector;
        [SerializeField] protected Joint joint;

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
        }

        public override void OnStartClient()
        {
            if (!isOwned && Owner != null)
            {
                col.isTrigger = true;
            }

            if (!NetworkServer.active)
                Destroy(joint);
        }


        public override void StartUsing()
        {
            if (NetworkServer.active)
            {
                FireLoop().Forget();
            }
            else
                StartUsingCmd();
        }

        public override void StopUsing()
        {
            if (NetworkServer.active)
            {
                cancellationToken?.Cancel();
            }
            else
                StopUsingCmd();
        }

        [Command] private void StartUsingCmd() => StartUsing();
        [Command] private void StopUsingCmd() => StopUsing();


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

            CancelUsing();

            if (disableOnDequip)
                gameObject.SetActive(false);
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

            if (Owner.TryFindAbillity<AbillityDataSessionManager>(out var sessionManager) && sessionManager.Has()
                                                                    && sessionManager.Get() is SessionManagerDeathmatch deathmatch)
            {
                deathmatch.AddToSpawned(projectile.gameObject);
            }
                
            if (shootEffector != null)
                shootEffector.Play();

            CurrentAmmo--;
        }

        protected async UniTaskVoid FireLoop()
        {
            cancellationToken?.ResetToken();
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
