using Mirror;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityDataFraction : AbillityData<Fraction>
    {
        [field: SyncVar]
        protected override Fraction Value { get; set; }


        public override void OnStopServer()
        {
            if (Value != null) 
                Value.Leave(Entity);
        }
    }
}
