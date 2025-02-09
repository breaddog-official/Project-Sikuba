using Scripts.SessionManagers;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Results
{
    public class ResultsTimeUI : MonoBehaviour
    {
        [SerializeField] protected SessionManagerDeathmatch sessionManager;
        [Space]
        [SerializeField] protected TMP_Text timeText;
        [SerializeField] protected string timeFormat = @"mm\:ss";

        public void DrawTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(sessionManager.GetMatchTime());

            if (timeText != null)
                timeText.text = time.ToString(timeFormat);
        }
    }
}
