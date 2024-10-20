using Mirror;

namespace Scripts.Gameplay
{
    public abstract class Projectile : NetworkBehaviour
    {
        /// <summary>
        /// Config must be <see cref="ProjectileConfig"/>
        /// </summary>
        public abstract void Initialize(object config);
    }
}
