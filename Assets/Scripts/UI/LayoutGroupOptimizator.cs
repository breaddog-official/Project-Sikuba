using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [AddComponentMenu("Layout Group Optimizator")]
    public class LayoutGroupOptimizator : MonoBehaviour
    {
        public enum EnableMode
        {
            Manual,
            OnEnable,
            OnAwake,
            Update,
        }

        [SerializeField] private EnableMode enableMode;

        private LayoutGroup layoutGroup;
        private uint updatesBeforeDisable;

        private void Awake()
        {
            if (layoutGroup == null && !TryGetComponent(out layoutGroup))
                return;

            if (enableMode == EnableMode.Manual)
            {
                layoutGroup.enabled = false;
            }
            else if (enableMode == EnableMode.OnAwake)
            {
                UpdateGroup();
            }
            else if (enableMode == EnableMode.Update)
            {
                layoutGroup.enabled = true;
            }
        }

        private void OnEnable()
        {
            if (enableMode == EnableMode.OnEnable)
            {
                UpdateGroup();
            }
        }

        private void Update()
        {
            if (layoutGroup == null)
                return;

            layoutGroup.enabled = enableMode == EnableMode.Update || updatesBeforeDisable > 0;

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
    }
}