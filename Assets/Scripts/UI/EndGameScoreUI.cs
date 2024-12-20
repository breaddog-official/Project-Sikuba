using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class EndGameScoreUI : MonoBehaviour
    {
        [SerializeField] protected SessionManagerDeathmatch sessionManager;
        [SerializeField] protected float calculateRate;
        [Space]
        [SerializeField] protected GameObject scoresPanel;
        [SerializeField] protected TMP_Text winFractionsText;
        [SerializeField] protected TMP_Text loseFractionsText;
        [SerializeField] protected TMP_Text timeText;

        private CancellationTokenSource cancellationToken;


        protected virtual void OnEnable()
        {
            sessionManager.OnStopMatch += StartRecalculatingScore;
        }

        protected virtual void OnDisable()
        {
            sessionManager.OnStopMatch -= StartRecalculatingScore;
            cancellationToken?.ResetToken();
        }



        protected virtual void CalculateScore()
        {
            string[] winners = GetNamesByState(SessionManagerDeathmatch.FractionState.Winner);
            string[] losers = GetNamesByState(SessionManagerDeathmatch.FractionState.Loser);
            TimeSpan time = TimeSpan.FromSeconds(sessionManager.GetMatchTime());

            if (winFractionsText != null)
                winFractionsText.text = GetEnumerableString(winners);

            if (loseFractionsText != null)
                loseFractionsText.text = GetEnumerableString(losers);

            if (timeText != null)
                timeText.text = $"{time.Hours}:{time.Minutes}:{time.Seconds}";
        }

        protected void StartRecalculatingScore() => RecalculateScore().Forget();
        protected async UniTaskVoid RecalculateScore()
        {
            scoresPanel.SetActive(true);
            cancellationToken ??= new CancellationTokenSource();

            while (scoresPanel.activeInHierarchy)
            {
                CalculateScore();
                await UniTask.Delay(calculateRate.ConvertSecondsToMiliseconds(), cancellationToken: cancellationToken.Token);
            }
        }



        protected static string GetEnumerableString(params string[] strings)
        {
            string text = string.Empty;
            for (int i = 0; i < strings.Length; i++)
            {
                text += $"{strings[i]}{(i + 1 == strings.Length ? string.Empty : ", ")}";
            }
            return text;
        }

        protected string[] GetNamesByState(SessionManagerDeathmatch.FractionState state)
        {
            HashSet<string> names = new();
            foreach(var pair in sessionManager.GetFractions())
            {
                if (pair.Value == state && pair.Key.TryFindByID(out Fraction fraction))
                {
                    names.Add(fraction.Name);
                }
            }
            return names.ToArray();
        }
    }
}