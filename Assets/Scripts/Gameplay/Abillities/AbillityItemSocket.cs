using Mirror;

namespace Scripts.Gameplay.Abillities
{
    /// <summary>
    /// Class for displaying and management an equipped item
    /// </summary>
    public class AbillityItemSocket : Abillity
    {
        public virtual Item EquippedItem { get; protected set; }

        public virtual Item LastEquippedItem { get; protected set; }

        /// <summary>
        /// Equips item
        /// </summary>
        [Server]
        public virtual void EquipItem(Item item)
        {
            if (EquippedItem != null)
                DropItem();

            item.netIdentity.AssignClientAuthority(connectionToClient);

            EquippedItem = item;
            EquippedItem.OnEquip(connectionToClient);
        }

        /// <summary>
        /// Drops the EquippedItem
        /// </summary>
        [Server]
        public virtual void DropItem()
        {
            if (EquippedItem == null)
                return;

            EquippedItem.netIdentity.RemoveClientAuthority();

            EquippedItem.OnDequip();

            LastEquippedItem = EquippedItem;
            EquippedItem = null;
        }


        public virtual bool HasItem() => EquippedItem != null;
    }
}