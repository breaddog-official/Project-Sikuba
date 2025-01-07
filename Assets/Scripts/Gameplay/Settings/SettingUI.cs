using UnityEngine;

namespace Scripts.Settings.UI
{
    public abstract class SettingUI : MonoBehaviour
    {
        [field: SerializeField] 
        public virtual string Name { get; protected set; }


        protected virtual void OnEnable()
        {
            SettingsManager.OnSettingsChanged += UpdateValue;
        }

        protected virtual void OnDisable()
        {
            SettingsManager.OnSettingsChanged -= UpdateValue;
        }



        protected abstract void UpdateValue(Settings oldSettings, Settings newSettings);
    }
}
