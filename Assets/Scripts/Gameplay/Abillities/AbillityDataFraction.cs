using Mirror;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityDataFraction : AbillityData<Fraction>
    {
        [field: SyncVar]
        protected override Fraction Value { get; set; }


        public override void Void()
        {
            if (Value != null)
                Value.Leave(Entity);
            
            base.Void();
        }

        public override void OnStopServer()
        {
            Void();
        }
    }
}
