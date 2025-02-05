using System;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.ColorHandlers
{
    [Serializable]
    public struct FractionColorHandler
    {
        public ColorHandler colorHandler;
        public FractionColor fractionColor;

        public readonly void SetColor(in Fraction fraction)
        {
            colorHandler.SetColor(fraction.GetColor(fractionColor));
        }
    }
}