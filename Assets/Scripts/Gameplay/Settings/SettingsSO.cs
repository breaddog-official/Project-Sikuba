using UnityEngine;

namespace Scripts.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Configs/Settings")]
    public class SettingsSO : ScriptableObject
    {
        [field: SerializeField]
        public Settings Settings { get; protected set; }


        public static explicit operator SettingsSO(Settings settings)
        {
            var instance = CreateInstance<SettingsSO>();
            instance.Settings = settings;

            return instance;
        }

        public static implicit operator Settings(SettingsSO settingsSO)
        {
            return settingsSO.Settings;
        }
    }
}