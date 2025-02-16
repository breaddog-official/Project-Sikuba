using UnityEngine;
using Scripts.Gameplay.Abillities;
using Scripts.Extensions;

namespace Scripts.Gameplay.Visuals
{
    public class VisualHealth : Visual<AbillityHealth>
    {
        [SerializeField] protected Transform healthBar;
        [SerializeField] protected GenVector3<bool> axis;
        [SerializeField] protected GenVector3<bool> inverse;



        protected void OnEnable()
        {
            Abillity.OnHealthChanged += UpdateHealth;
        }

        protected void OnDisable()
        {
            Abillity.OnHealthChanged += UpdateHealth;
        }



        public void UpdateHealth(float oldHealth, float newHealth)
        {
            float health = newHealth / Abillity.MaxHealth;
            float remainder = 1f - health;

            Vector3 scale = new(health, health, health);
            scale = Vector3.Scale(scale, axis.ToInteger());

            Vector3 position = new(remainder, remainder, remainder);
            position = Vector3.Scale(position, inverse.ToInteger(FalsePresentation.MinusOne));

            healthBar.position = position;
            healthBar.localScale = scale;
        }
    }
}
