﻿namespace Scripts.Gameplay
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void Heal(float amount);
    }
}