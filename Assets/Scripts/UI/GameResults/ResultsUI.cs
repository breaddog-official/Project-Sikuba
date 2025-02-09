using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using Scripts.UI.Tabs;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI.Results
{
    public class ResultsUI : MonoBehaviour
    {
        [SerializeField] protected SessionManagerDeathmatch sessionManager;
        [SerializeField] protected TabsTranslaterDOTween tabsTranslater;
        [SerializeField] protected float drawRate;
        [Space]
        [SerializeField] protected CanvasGroup resultsPanel;

        [field: SerializeField, Space]
        public UnityEvent OnDrawResults { get; protected set; }
        
        private CancellationTokenSource cancellationToken;



        protected virtual void OnEnable()
        {
            sessionManager.OnStopMatch += StartRedrawingResults;
        }

        protected virtual void OnDisable()
        {
            sessionManager.OnStopMatch -= StartRedrawingResults;
            cancellationToken?.ResetToken();
        }



        protected virtual void DrawResults()
        {
            OnDrawResults?.Invoke();
        }



        protected void StartRedrawingResults() => RedrawResults().Forget();

        protected async UniTaskVoid RedrawResults()
        {
            tabsTranslater.SwitchTab(resultsPanel);
            cancellationToken ??= new CancellationTokenSource();

            while (resultsPanel == tabsTranslater.GetCurrentTabGroup())
            {
                DrawResults();
                await UniTask.Delay(drawRate.ConvertSecondsToMiliseconds(), cancellationToken: cancellationToken.Token);
            }
        }

    }
}