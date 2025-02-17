using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Effector : NetworkBehaviour
    {
        public virtual void Play()
        {
            PlayEffect();
        }


        protected abstract void PlayEffect();
    }
}
