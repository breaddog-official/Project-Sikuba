using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [AddComponentMenu("Layout Group Optimizator")]
    public class LayoutGroupOptimizator : MonoBehaviour
    {
        [Flags]
        public enum EnableMode
        {
            Manual = 0,
            OnEnable = 1 << 0,
            OnStart = 1 << 1,
            Update = 1 << 2,
        }

        [SerializeField, EnumFlags] private EnableMode enableMode;

        private LayoutGroup layoutGroup;
        private uint updatesBeforeDisable;

        private void Start()
        {
            if (layoutGroup == null && !TryGetComponent(out layoutGroup))
                return;
            

            if (HasFlag(EnableMode.OnStart))
            {
                UpdateGroup();
            }

            if (HasFlag(EnableMode.Update))
            {
                layoutGroup.enabled = true;
            }

            if (HasFlag(EnableMode.Manual))
            {
                layoutGroup.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (HasFlag(EnableMode.OnEnable))
            {
                UpdateGroup();
            }
        }

        private void Update()
        {
            if (layoutGroup == null)
                return;

            layoutGroup.enabled = HasFlag(EnableMode.Update) || updatesBeforeDisable > 0;

            if (updatesBeforeDisable > 0)
                updatesBeforeDisable--;
            
        }

        public void UpdateGroup()
        {
            if (layoutGroup == null)
                return;

            updatesBeforeDisable++;
            layoutGroup.enabled = true;
        }


        private bool HasFlag(EnableMode flag) => (enableMode & flag) == flag;
    }
}