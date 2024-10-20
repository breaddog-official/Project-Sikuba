using UnityEngine;

namespace Scripts.Gameplay
{
    public class ProjectileBullet : Projectile
    {
        public ProjectileBulletConfig Config { get; protected set; }

        /// <summary>
        /// Config must be <see cref="ProjectileBulletConfig"/>
        /// </summary>
        public override void Initialize(object config)
        {
            Config = config as ProjectileBulletConfig;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Config.Damage);
            }
        }
    }
}
