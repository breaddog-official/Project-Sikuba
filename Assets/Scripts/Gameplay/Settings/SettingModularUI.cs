using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Settings.UI
{
    public sealed class SettingModularUI : SettingUI
    {
        enum SettingType
        {
            Toggle,
            Slider,
            Dropdown,
            InputField
        }

        [Space]
        [SerializeField] private SettingType settingType;

        [ShowIf(nameof(settingType), SettingType.Toggle)]
        [SerializeField] private Toggle toggle;

        [ShowIf(nameof(settingType), SettingType.Slider)]
        [SerializeField] private Slider slider;

        [ShowIf(nameof(settingType), SettingType.Dropdown)]
        [SerializeField] private TMP_Dropdown dropdown;

        [ShowIf(nameof(settingType), SettingType.InputField)]
        [SerializeField] private TMP_InputField inputField;


        protected override void UpdateValue()
        {
            switch (settingType)
            {
                case SettingType.Toggle:
                    toggle.isOn = (bool)Value;
                    break;

                case SettingType.Slider:
                    slider.value = (float)Value;
                    break;

                case SettingType.Dropdown:
                    dropdown.value = (int)Value;
                    break;

                case SettingType.InputField:
                    inputField.text = (string)Value;
                    break;
            }
        }


        public void SetInt(int value) => SetValue(value);
        public void SetFloat(float value) => SetValue(value);
        public void SetString(string value) => SetValue(value);
        public void SetBool(bool value) => SetValue(value);
    }
}
