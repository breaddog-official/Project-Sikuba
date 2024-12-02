using Mirror;
using UnityEngine;
using Scripts.MonoCache;

namespace Scripts.Network
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Network/Network Collider")]
    public class NetworkCollider : NetworkBehaviour, IMonoCacheUpdate
    {
        [SyncVar(hook = nameof(ApplyChanges))]
        private bool isTrigger;


        public Behaviour Behaviour => this;

        private Collider col;



        protected virtual void Awake()
        {
            MonoCacher.Registrate(this);

            col = GetComponent<Collider>();
        }

        public virtual void UpdateCached()
        {
            if (syncDirection == SyncDirection.ServerToClient && !isServer)
                return;

            if (syncDirection == SyncDirection.ClientToServer && (!isClient || !isOwned))
                return;

            isTrigger = col.isTrigger;
        }


        protected virtual void ApplyChanges(bool oldValue, bool newValue)
        {
            col.isTrigger = newValue;
        }

        // Need for enabled checkbox in inspector
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}
