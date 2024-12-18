using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [ExecuteAlways]
    public class LayoutPrefferedSize : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LayoutGroup group;
        [Space]
        [SerializeField] private bool overrideX;
        [SerializeField] private bool overrideY;

        private void Start()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (group == null)
                group = GetComponent<LayoutGroup>();
        }

        private void LateUpdate()
        {
            if (rectTransform == null || group == null || !group.enabled)
                return;

            if (!overrideX && !overrideY)
                return;

            float x = overrideX ? group.preferredWidth : rectTransform.sizeDelta.x;
            float y = overrideY ? group.preferredHeight : rectTransform.sizeDelta.y;

            rectTransform.sizeDelta = new(x, y);
        }
    }
}
