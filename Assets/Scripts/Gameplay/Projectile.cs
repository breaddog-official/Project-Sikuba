using Mirror;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay
{
    public abstract class Projectile : NetworkBehaviour
    {
        public abstract void Initialize(Entity sender);
    }
}
