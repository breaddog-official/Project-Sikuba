using NaughtyAttributes;
using UnityEngine;
using System;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerParticleSystem : ColorHandler
    {
        [SerializeField] private bool overrideParticle;
        [ShowIf(nameof(overrideParticle))]
        [SerializeField] private ParticleSystem overridedParticle;
        private ParticleSystem cachedParticle;


        public ParticleSystem Particle => overrideParticle ? overridedParticle : cachedParticle ??= GetComponent<ParticleSystem>();



        public override void SetColor(Color color)
        {
            if (Particle == null)
                throw new ArgumentNullException(nameof(Particle));

            var main = Particle.main;
            main.startColor = new(color);
        }

        public override Color GetColor()
        {
            if (Particle == null)
                throw new ArgumentNullException(nameof(Particle));

            return Particle.main.startColor.color;
        }
    }
}