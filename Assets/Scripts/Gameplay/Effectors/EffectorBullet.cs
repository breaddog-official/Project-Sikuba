using NaughtyAttributes;
using Scripts.Gameplay.Fractions;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorBullet : EffectorExtension
    {
        [Space]
        [SerializeField] private ProjectileBullet bullet;
        [SerializeField] private bool changeParticleColor;
        [ShowIf(nameof(changeParticleColor))]
        [SerializeField] private FractionColor fractionColor;


        protected override void PlayParticle(ParticleSystem particle)
        {
            base.PlayParticle(particle);

            if (changeParticleColor && bullet.Fraction != null)
            {
                var main = particle.main;
                main.startColor = new(bullet.Fraction.GetColor(fractionColor));
            }
        }
    }
}