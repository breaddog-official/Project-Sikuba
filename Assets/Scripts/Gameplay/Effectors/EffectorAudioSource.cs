using UnityEngine;

namespace Scripts.Gameplay
{
    public class EffectorAudioSource : EffectorExtension<AudioSource>
    {
        protected override void ExecuteEffect(AudioSource obj)
        {
            obj.Play();
        }
    }
}
