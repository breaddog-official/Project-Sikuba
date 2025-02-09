using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Scripts.Extensions;
using NaughtyAttributes;

namespace Scripts.UI.Tabs
{
    public abstract class TabsTranslater<T> : MonoBehaviour where T : Tab
    {
        [SerializeField] private bool showInitialOnStart = true;
        [ShowIf(nameof(CanGetTabsGroups))]
        [SerializeField, Dropdown(nameof(GetTabsGroups))] private CanvasGroup initialTab;

        private CancellationTokenSource cancallationToken;
        private bool switchingTab;
        private T currentTab;


        private void Start()
        {
            currentTab = FindTab(initialTab);

            if (showInitialOnStart)
                SwitchTab(initialTab);
        }

        public virtual void SwitchTab(CanvasGroup tabGroup) => SwitchTab(FindTab(tabGroup));

        public virtual void SwitchTab(T tab) => SwitchTab(currentTab, tab);
        public virtual void ShowTab() => SwitchTab(null, currentTab);
        public virtual void HideTab() => SwitchTab(currentTab, null, false);

        public virtual async void SwitchTab(T from, T to, bool withSetCurrentTab = true)
        {
            if (switchingTab)
                return;

            switchingTab = true;

            cancallationToken?.ResetToken();
            cancallationToken = new();

            await VisualizeSwitchTabs(from, to, cancallationToken.Token);

            if (withSetCurrentTab)
                currentTab = to;

            switchingTab = false;
        }

        public T GetCurrentTab() => currentTab;
        public CanvasGroup GetCurrentTabGroup() => currentTab.canvasGroup;



        public virtual void CancelSwitching()
        {
            switchingTab = false;
            cancallationToken?.Cancel();
        }

        public virtual T FindTab(CanvasGroup canvasGroup)
        {
            return GetTabs().Where(t => t.canvasGroup == canvasGroup).FirstOrDefault();
        }

        private void OnDestroy()
        {
            CancelSwitching();
        }


        public abstract UniTask VisualizeSwitchTabs(T oldTab, T newTab, CancellationToken token = default);

        protected abstract IReadOnlyCollection<T> GetTabs();

        protected virtual CanvasGroup[] GetTabsGroups()
        {
            return GetTabs().Select(tab => tab.canvasGroup).ToArray();
        }

        protected virtual bool CanGetTabsGroups()
        {
            IReadOnlyCollection<T> tabs = GetTabs();
            return tabs != null && tabs.Count > 0;
        }
    }
}
