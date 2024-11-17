using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Effector : NetworkBehaviour
    {
        [Command]
        public void Play()
        {
            PlayEffect();
        }

        [ClientRpc]
        protected virtual void PlayEffect() { }
    }
}
