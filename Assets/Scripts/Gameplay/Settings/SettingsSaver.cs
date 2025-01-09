using Cysharp.Threading.Tasks;
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
        [SerializeField] private bool scriptableObjectSettings;

        [HideIf(nameof(scriptableObjectSettings))]
        [SerializeField] private Settings defaultSettings;

        [ShowIf(nameof(scriptableObjectSettings))]
        [SerializeField] private SettingsSO defaultSettingsSO;

        public string SavePath => Path.Combine(Application.persistentDataPath, "Configs", "Settings.json");

        private Settings DefaultSettings => scriptableObjectSettings ? defaultSettingsSO.Settings : defaultSettings;



        private void Awake()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            if (SettingsManager.Settings == null)
            {
                LoadSettings();
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
