using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Effector : NetworkBehaviour
    {
        public void Play()
        {
            PlayEffect();
        }

        [Server]
        public void PlayOnClients()
        {
            RpcPlayOnClients();
        }


        [ClientRpc]
        public void RpcPlayOnClients() => PlayEffect();
        protected abstract void PlayEffect();

    }
}
