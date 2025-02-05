using System;
using Scripts.Extensions;
using Scripts.Gameplay.ColorHandlers;
using Scripts.Gameplay.Fractions;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectionFractionUI : MonoBehaviour, IInitializable
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private FractionColorHandler[] colorHandlers;

        private bool isInitialized;
        public bool IsInitialized => isInitialized;

        public Fraction Fraction { get; private set; }

        private Action<Fraction> OnSelectFraction;


        public bool Initialize()
        {
            if (isInitialized.CheckInitialization())
                return false;

            return true;
        }

        public bool Initialize(Fraction fraction, Action<Fraction> onSelectFraction = null)
        {
            if (!Initialize())
                return false;

            OnSelectFraction = onSelectFraction;
            Fraction = fraction;

            if (Fraction != null)
            {
                nameText.SetText(Fraction.Name);

                foreach(var handler in colorHandlers)
                {
                    handler.SetColor(Fraction);
                }
            }

            return true;
        }

        public void Click()
        {
            OnSelectFraction?.Invoke(Fraction);
        }

        public static explicit operator Fraction(SelectionFractionUI fractionUI) => fractionUI.Fraction;
    }
}