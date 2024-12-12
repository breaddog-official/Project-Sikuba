using Mirror;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityDataFraction : AbillityData<Fraction>
    {
        [field: SyncVar]
        protected override Fraction Value { get; set; }
    }
}
