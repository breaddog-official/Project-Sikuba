using UnityEngine;
using UnityEngine.Audio;

namespace Scripts.Settings
{
    public class SObserverVolume : SettingObserver
    {
        [SerializeField] protected string Name;
        [SerializeField] protected AudioMixerGroup mixer;
        [SerializeField] protected string volumeParameterName;

        protected override void UpdateValue()
        {
            mixer.audioMixer.SetFloat(volumeParameterName, (float)SettingsManager.GetSetting(Name));
        }
    }
}
