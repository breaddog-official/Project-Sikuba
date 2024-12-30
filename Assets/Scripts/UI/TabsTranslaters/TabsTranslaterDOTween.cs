using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Scripts.UI.Tabs.TabsTranslaterDOTween;

namespace Scripts.UI.Tabs
{
    public class TabsTranslaterDOTween : TabsTranslater<TabDOTween>
    {
        [SerializeField] private TabDOTween[] tabs;

        public override async UniTask VisualizeSwitchTabs(TabDOTween oldTab, TabDOTween newTab, CancellationToken token = default)
        {
            if (oldTab != null)
            {
                await oldTab.canvasGroup.DOFade(0.0f, oldTab.fadeDuration)
                                    .SetEase(oldTab.ease)
                                    .WithCancellation(token);

                oldTab.canvasGroup.gameObject.SetActive(false);
            }

            if (newTab != null)
            {
                newTab.canvasGroup.gameObject.SetActive(true);

                await newTab.canvasGroup.DOFade(1.0f, newTab.showDuration)
                                    .SetEase(newTab.ease)
                                    .WithCancellation(token);
            }
        }

        protected override IReadOnlyCollection<TabDOTween> GetTabs() => tabs;

        [Serializable]
        public class TabDOTween : Tab
        {
            public float fadeDuration = 1.0f;
            public float showDuration = 1.0f;
            public Ease ease = Ease.Linear;
        }
    }
}
