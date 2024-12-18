using Scripts.Gameplay.Fractions;
using Scripts.MonoCache;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class FractionDeathsUI : MonoBehaviour, IMonoCacheUpdate
    {
        [SerializeField] protected FractionDeathmatch fraction;
        [SerializeField] protected TMP_Text counterText;

        public Behaviour Behaviour => this;


        public virtual void UpdateCached()
        {
            if (counterText == null || fraction == null)
                return;

            counterText.SetText(fraction.Deaths.ToString());
        }
    }
}