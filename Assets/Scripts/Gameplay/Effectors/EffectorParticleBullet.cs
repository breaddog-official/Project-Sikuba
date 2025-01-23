using NaughtyAttributes;
using Scripts.Gameplay.ColorHandlers;
using Scripts.Gameplay.Fractions;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorParticleBullet : EffectorParticle
    {
        [Space]
        [SerializeField] private ProjectileBullet bullet;
        [Space]
        [SerializeField] private bool changeParticleColor;
        [ShowIf(nameof(changeParticleColor))]
        [SerializeField] private FractionColor fractionColor;


        protected override void ExecuteEffect(ParticleSystem particle)
        {
            base.ExecuteEffect(particle);

            if (changeParticleColor && bullet.Fraction != null && particle.TryGetComponent(out ColorHandler colorHandler))
            {
                colorHandler.SetColor(bullet.Fraction.GetColor(fractionColor));
            }
        }
    }
}