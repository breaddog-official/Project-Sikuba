using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Settings
{
    public abstract class SettingHandler : SettingObserver
    {
        [field: SerializeField] 
        public virtual string Name { get; protected set; }

        public virtual object Setting => SettingsManager.GetSetting(Name);


        protected virtual void SetSetting(object value)
        {
            SettingsManager.SetSetting(Name, value);
        }

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
    /// Generic version of <see cref="SettingHandler"/>
    /// </summary>
    public abstract class SettingHandler<T> : SettingHandler
    {
        public new T Setting => SettingsManager.GetSetting<T>(Name);

        public virtual void SetValue(T value)
        {
            base.SetSetting(value);
        }
    }
}
