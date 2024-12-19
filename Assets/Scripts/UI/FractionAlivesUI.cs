using Mirror;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using Scripts.MonoCache;
using Scripts.SessionManagers;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class FractionAlivesUI : MonoBehaviour, IMonoCacheUpdate
    {
        [SerializeField] protected FractionDeathmatch fraction;
        [SerializeField] protected TMP_Text counterText;

        public Behaviour Behaviour => this;

        protected virtual void Awake()
        {
            MonoCacher.Registrate(this);
        }

        public virtual void UpdateCached()
        {
            if (counterText == null || fraction == null)
                return;

            counterText.SetText(fraction.Alives.Count.ToString());
        }
    }
}