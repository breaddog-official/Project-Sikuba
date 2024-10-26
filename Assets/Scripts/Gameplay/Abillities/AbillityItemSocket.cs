using Mirror;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityItemSocket : Abillity
    {
        public virtual Item EquippedItem { get; protected set; }

        /// <summary>
        /// Equips gun
        /// </summary>
        [Command]
        public virtual void EquipItem(Item item)
        {
            if (EquippedItem != null)
                DropItem();

            item.netIdentity.AssignClientAuthority(connectionToClient);

            EquippedItem = item;
        }

        /// <summary>
        /// Drops the EquippedGun
        /// </summary>
        [Command]
        public virtual void DropItem()
        {
            if (EquippedItem == null)
                return;

            EquippedItem.netIdentity.RemoveClientAuthority();

            EquippedItem = null;
        }
    }
}