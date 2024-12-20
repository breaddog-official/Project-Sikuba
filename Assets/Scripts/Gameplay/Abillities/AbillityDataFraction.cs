using Mirror;
using Scripts.Gameplay.Fractions;
using System;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityDataFraction : AbillityData<Fraction>
    {
        [field: SyncVar(hook = nameof(OnChange))]
        protected override Fraction Value { get; set; }

        public event Action<Fraction> OnFractionChange;


        public override void Set(Fraction value)
        {
            base.Set(value);

            OnChange();
        }

        public override void Void()
        {
            if (Value != null)
                Value.Leave(Entity);
            
            base.Void();

            OnChange();
        }

        private void OnChange(Fraction oldValue = null, Fraction newValue = null)
        {
            OnFractionChange?.Invoke(Value);
        }

        public override void OnStopServer()
        {
            Void();
        }
    }
}
