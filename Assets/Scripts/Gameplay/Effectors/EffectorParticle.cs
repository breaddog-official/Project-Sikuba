using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorParticle : EffectorExtension<ParticleSystem>
    {
        protected override void ExecuteEffect(ParticleSystem obj)
        {
            obj.Play();
        }
    }
}
