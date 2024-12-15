using Scripts.Gameplay.Fractions;
using UnityEngine;

namespace Scripts.UI
{
    public class FractionSelectorUI : MonoBehaviour
    {
        public void JoinTo(Fraction fraction)
        {
            fraction.RequestToJoin();
        }
    }
}