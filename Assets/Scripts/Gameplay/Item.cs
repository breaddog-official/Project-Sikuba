﻿using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Item : NetworkBehaviour
    {
        /// <summary>
        /// Start using an item (by default left mouse button press)
        /// </summary>
        public abstract void StartUsing();
        /// <summary>
        /// Stop using an item (by default left mouse button cancel)
        /// </summary>
        public abstract void StopUsing();


        /// <summary>
        /// Cancel using an item. It is necessary, for example, when starting a cutscene <br /> <br />
        /// 
        /// By default calls <see cref="StopUsing"/>
        /// </summary>
        public virtual void CancelUsing() => StopUsing();


        public abstract void OnEquip(NetworkConnectionToClient conn);
        public abstract void OnDequip();
    }
}