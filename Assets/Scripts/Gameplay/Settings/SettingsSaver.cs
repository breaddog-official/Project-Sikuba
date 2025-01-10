using System;
using NaughtyAttributes;
using Scripts.SaveManagement;
using System.IO;
using UnityEngine;

namespace Scripts.Settings
{
    public class SettingsSaver : MonoBehaviour
    {
        [SerializeField] private bool saveOnChange = true;
        [SerializeField] private bool dontDestroyOnLoad;
        [Space]
        [SerializeField] private Serializer serializer;

        [Space]
        [SerializeField] private bool autoSetDefaultSettings;

        [ShowIf(nameof(autoSetDefaultSettings))]
        [SerializeField] private bool scriptableObjectSettings;

        [HideIf(EConditionOperator.Or, nameof(scriptableObjectSettings), nameof(DontSetDefaultSettings))]
        [SerializeField] private Settings defaultSettings;

        [ShowIf(EConditionOperator.And, nameof(scriptableObjectSettings), nameof(autoSetDefaultSettings))]
        [SerializeField] private SettingsSO defaultSettingsSO;


        public string SavePath => Path.Combine(Application.persistentDataPath, "Configs", "Settings.nahuy");

        private Settings DefaultSettings => scriptableObjectSettings ? defaultSettingsSO.Settings : defaultSettings;

        private bool DontSetDefaultSettings => !autoSetDefaultSettings;


        private void Awake()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            print(SavePath);
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



        private void OnEnable()
        {
            if (saveOnChange)
                SettingsManager.OnSettingsChanged += SaveSettings;
        }

        private void OnDisable()
        {
            SettingsManager.OnSettingsChanged -= SaveSettings;
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
