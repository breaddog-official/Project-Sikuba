using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using UnityEngine;

namespace Scripts.UI
{
    public class FractionSelector : MonoBehaviour
    {
        [SerializeField] private SessionManagerCommandMatch sessionManager;


        public void SelectTeam(Fraction fraction)
        {
            CommandMatchConfig config = new() 
            {
                fraction = fraction
            };

            sessionManager.SendRequestToSpawn(config);
        }
    }
}