using System;

namespace Scripts.Settings
{
    public static class SettingsManager
    {
        public static Settings Settings { get; private set; }

        /// <summary>
        /// Old settings and new settings
        /// </summary>
        public static event Action<Settings, Settings> OnSettingsChanged;



        public static void SetSettings(Settings settings)
        {
            OnSettingsChanged?.Invoke(settings, Settings);
            Settings = settings;
        }



        /// <summary>
        /// Safely sets setting
        /// </summary>
        public static void SetSetting(string name, object value)
        {
            Settings?.SetValue(name, value);
        }

        /// <summary>
        /// Safely gets setting
        /// </summary>
        public static object GetSetting(string name)
        {
            return Settings?.GetValue(name);
        }

        /// <summary>
        /// Safely gets setting as T
        /// </summary>
        public static T GetSetting<T>(string name)
        {
            return Settings.GetValue<T>(name) ?? default;
        }
    }
}
