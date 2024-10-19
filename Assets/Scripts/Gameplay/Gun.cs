using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Gun : NetworkBehaviour
    {
        public abstract uint CurrentAmmo { get; protected set; }
        public abstract uint MaxAmmo { get; protected set; }

        public abstract bool IsReloadable { get; protected set; }


        public abstract bool StartFire();
        public abstract bool StopFire();

        public virtual bool CancelFire() => StopFire();

        public virtual bool Reload()
        {
            if (!IsReloadable)
                return false;

            CurrentAmmo = MaxAmmo;

            return true;
        }
    }
}