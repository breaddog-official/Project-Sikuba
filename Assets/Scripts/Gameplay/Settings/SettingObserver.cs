using UnityEngine;

namespace Scripts.Settings
{
    public abstract class SettingObserver : MonoBehaviour
    {
        [SerializeField] protected bool dontDestroyOnLoad;


        protected virtual void Awake()
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnEnable()
        {
            SettingsManager.OnSettingsChanged += UpdateValue;
            UpdateValue();
        }

        protected virtual void OnDisable()
        {
            SettingsManager.OnSettingsChanged -= UpdateValue;
        }


        protected abstract void UpdateValue();
    }
}
