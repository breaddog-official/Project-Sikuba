using Scripts.Extensions;
using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Results
{
    public abstract class ResultsFractionsUI : MonoBehaviour
    {
        [SerializeField] protected SessionManagerDeathmatch sessionManager;
        [SerializeField] protected SessionManagerDeathmatch.FractionState fractionState = SessionManagerDeathmatch.FractionState.NotStated;


        public abstract void DrawFractions();


        protected Fraction[] GetFractions()
        {
            HashSet<Fraction> fractions = new();
            foreach (var pair in sessionManager.GetFractions())
            {
                if (pair.Value == fractionState && pair.Key.TryFindByID(out Fraction fraction))
                {
                    fractions.Add(fraction);
                }
            }
            return fractions.ToArray();
        }
    }

    public class ResultsFractionsTextUI : ResultsFractionsUI
    {
        [Space]
        [SerializeField] protected TMP_Text fractionsText;


        public override void DrawFractions()
        {
            string[] fractions = GetFractions().Select(f => f.Name).ToArray();

            if (fractionsText != null)
                fractionsText.text = GetEnumerableString(fractions);
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
    }
}
