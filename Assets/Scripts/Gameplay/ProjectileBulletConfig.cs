namespace Scripts.Gameplay
{
    public class ProjectileBulletConfig : ProjectileConfig
    {
        public readonly float Lifetime;

        public ProjectileBulletConfig(float damage, float lifetime) : base(damage)
        {
            Lifetime = lifetime;
        }
    }
}
