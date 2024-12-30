using Mirror;

namespace Scripts.Gameplay.Abillities
{
    /// <summary>
    /// Class for displaying and management an equipped item
    /// </summary>
    public class AbillityItemSocket : Abillity
    {
        [field: SyncVar]
        public virtual Item EquippedItem { get; protected set; }

        [field: SyncVar]
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

            EquippedItem.SetOwner(Entity);
            EquippedItem.OnEquip();
        }

        /// <summary>
        /// Drops the EquippedItem
        /// </summary>
        [Server]
        public virtual void DropItem()
        {
            if (EquippedItem == null)
                return;

            EquippedItem.SetOwner(null);
            EquippedItem.OnDequip();

            // After on dequip item may be destroyed
            if (EquippedItem == null)
                return;

            EquippedItem.netIdentity.RemoveClientAuthority();

            LastEquippedItem = EquippedItem;
            EquippedItem = null;
        }


        public virtual bool HasItem() => EquippedItem != null;


        public override void ResetState()
        {
            base.ResetState();

            DropItem();
        }
    }
}