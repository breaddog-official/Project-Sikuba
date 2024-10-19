using Mirror;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityGunSocket : Abillity
    {
        public Gun EquippedGun { get; protected set; }

        /// <summary>
        /// Equips gun
        /// </summary>
        [Command]
        public virtual void EquipGun(Gun gun)
        {
            if (EquippedGun != null)
                DropGun();

            gun.netIdentity.AssignClientAuthority(connectionToClient);

            EquippedGun = gun;
        }

        /// <summary>
        /// Drops the EquippedGun
        /// </summary>
        [Command]
        public virtual void DropGun()
        {
            if (EquippedGun == null)
                return;

            EquippedGun.netIdentity.AssignClientAuthority(null);

            EquippedGun = null;
        }


        public virtual Gun GetEquippedGun() => EquippedGun;
    }
}