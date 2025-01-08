using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Settings.UI
{
    public abstract class SettingUI : MonoBehaviour
    {
        [field: SerializeField] 
        public virtual string Name { get; protected set; }

        public virtual object Value => SettingsManager.GetSetting(Name);



        protected virtual void OnEnable()
        {
            SettingsManager.OnSettingsChanged += UpdateValue;
        }

        protected virtual void OnDisable()
        {
            SettingsManager.OnSettingsChanged -= UpdateValue;
        }



        protected virtual void SetValue(object value)
        {
            SettingsManager.SetSetting(Name, value);
        }


        protected abstract void UpdateValue();

        #region Editor
#if UNITY_EDITOR

        [field: Dropdown(nameof(GetSettingsNamesEditor)), OnValueChanged(nameof(SetSettingEditor))]
        public string setName;

        public const string default_dropdown_item = "Select setting";


        public List<string> GetSettingsNamesEditor()
        {
            List<string> strings = typeof(Settings).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                               .Select(f => f.Name)
                                               .ToList();
            strings.Insert(0, default_dropdown_item);
            return strings;
        }

        public void SetSettingEditor()
        {
            if (setName == default_dropdown_item)
                return;

            Name = setName;
            setName = default_dropdown_item;
        }

#endif
        #endregion
    }

    /// <summary>
    /// Generic version of <see cref="SettingUI"/>
    /// </summary>
    public abstract class SettingUI<T> : SettingUI
    {
        public new T Value => SettingsManager.GetSetting<T>(Name);

        public virtual void SetValue(T value)
        {
            base.SetValue(value);
        }
    }
}
