using Scripts.Gameplay.Items;
using Mirror;
using Scripts.Gameplay.Visuals;
using System;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityArms : Abillity
    {
        public Item CurrentItem { get; private set; }


        public event Action OnEquip;
        public event Action OnThrow;


        [Server]
        public void EquipItem(Item item)
        {
            if (CurrentItem != null)
                ThrowItem();

            CurrentItem = item;


            OnEquip?.Invoke();
        }

        [Server]
        public void ThrowItem()
        {
            CurrentItem = null;


            OnThrow?.Invoke();
        }
    }
}
namespace Scripts.Gameplay.Items
{
    public class Item : NetworkBehaviour
    {
        [Server]
        public void ChangeAuthority(NetworkConnectionToClient conn)
        {
            netIdentity.AssignClientAuthority(conn);
        }
    }
}
