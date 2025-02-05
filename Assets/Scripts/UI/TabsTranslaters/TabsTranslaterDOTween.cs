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
        [SerializeField] private bool fadeTogether;

        public override async UniTask VisualizeSwitchTabs(TabDOTween oldTab, TabDOTween newTab, CancellationToken token = default)
        {
            UniTask oldTabFade = UniTask.CompletedTask;
            UniTask newTabFade = UniTask.CompletedTask;

            if (oldTab != null)
            {
                oldTabFade = oldTab.canvasGroup.DOFade(0.0f, oldTab.fadeDuration)
                                    .SetEase(oldTab.ease)
                                    .OnComplete(() => oldTab.canvasGroup.gameObject.SetActive(false))
                                    .WithCancellation(token);


                if (!fadeTogether)
                {
                    await oldTabFade;
                }
            }

            if (newTab != null)
            {
                newTab.canvasGroup.gameObject.SetActive(true);

                newTabFade = newTab.canvasGroup.DOFade(1.0f, newTab.showDuration)
                                    .SetEase(newTab.ease)
                                    .WithCancellation(token);

                if (!fadeTogether)
                {
                    await newTabFade;
                }
            }

            if (fadeTogether)
            {
                await UniTask.WhenAll(oldTabFade, newTabFade);
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
