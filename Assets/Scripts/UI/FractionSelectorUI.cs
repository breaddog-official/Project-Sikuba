using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using UnityEngine;

namespace Scripts.UI
{
    public class FractionSelectorUI : MonoBehaviour
    {
        [SerializeField] private SessionManagerCommandMatch sessionManager;


        public void JoinTo(Fraction fraction)
        {
            fraction.Join();
        }
    }
}