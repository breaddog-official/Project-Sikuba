using System.Linq;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Results
{
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
