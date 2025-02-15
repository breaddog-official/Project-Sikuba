using Scripts.Extensions;
using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using System.Collections.Generic;
using System.Linq;
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
}
