using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Effector : NetworkBehaviour
    {
        [Server]
        public void Play()
        {
            PlayEffect();
        }


        protected abstract void PlayEffect();
    }
}
