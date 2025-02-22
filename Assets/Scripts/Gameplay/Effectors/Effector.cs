using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Effector : NetworkBehaviour
    {
        public virtual void Play()
        {
            PlayEffect();
        }

        [ClientRpc]
        protected virtual void PlayOnClients()
        {
            PlayEffect();
        }


        protected abstract void PlayEffect();
    }
}
