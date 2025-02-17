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

        Vector3 cachedPosition;
        Vector3 cachedScale;

        
        protected virtual void Awake()
        {
            cachedPosition = healthBar.localPosition;
            cachedScale = healthBar.localScale;
        }



        protected virtual void OnEnable()
        {
            Abillity.OnHealthChanged += UpdateHealth;
        }

        protected virtual void OnDisable()
        {
            Abillity.OnHealthChanged -= UpdateHealth;
        }



        public void UpdateHealth(float oldHealth, float newHealth)
        {
            float health = newHealth / Abillity.MaxHealth;
            float remainder = 1f - health;
            
            Vector3 scale = Vector3.Scale(cachedScale, axis.ToVector(truePresent: health, falsePresent: 1));

            Vector3 position = new(remainder, remainder, remainder);
            position = Vector3.Scale(position, axis.ToVector(0));
            position = Vector3.Scale(position, inverse.ToVector(-1));

            healthBar.localPosition = cachedPosition + position;
            healthBar.localScale = scale;
        }
    }
}
