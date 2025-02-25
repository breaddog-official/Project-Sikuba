using NaughtyAttributes;
using Scripts.SaveManagement;
using System.IO;
using UnityEngine;

namespace Scripts.Settings
{
    public class SettingsSaver : SettingObserver
    {
        [SerializeField] private bool saveOnChange = true;
        [Space]
        [SerializeField] private Serializer serializer;

        [Space]
        [SerializeField] private bool autoSetDefaultSettings;

        [ShowIf(nameof(autoSetDefaultSettings))]
        [SerializeField] private bool scriptableObjectSettings = true;

        [HideIf(EConditionOperator.Or, nameof(scriptableObjectSettings), nameof(DontSetDefaultSettings))]
        [SerializeField] private Settings defaultSettings;

        [ShowIf(EConditionOperator.And, nameof(scriptableObjectSettings), nameof(autoSetDefaultSettings))]
        [SerializeField] private SettingsSO defaultSettingsSO;


        public string SavePath => Path.Combine(Application.persistentDataPath, "Configs", "Settings.nahuy");

        private Settings DefaultSettings => scriptableObjectSettings ? defaultSettingsSO : defaultSettings;

        private bool DontSetDefaultSettings => !autoSetDefaultSettings;


        protected void Start()
        {
            if (SettingsManager.Settings == null)
            {
                if (SaveManager.Exists(SavePath))
                {
                    LoadSettings();
                }
                else if (autoSetDefaultSettings && DefaultSettings != null)
                {
                    SettingsManager.SetSettings(DefaultSettings);
                    SaveSettings();
                }
            }
        }

        protected override void UpdateValue()
        {
            if (saveOnChange)
                SaveSettings();
        }


        public async void SaveSettings()
        {
            await SaveManager.SerializeAndSaveAsync(SettingsManager.Settings, SavePath, serializer);
        }

        public async void LoadSettings()
        {
            SettingsManager.SetSettings(await SaveManager.LoadAndDeserializeAsync<Settings>(SavePath, serializer));
        }
    }
}
