using Mirror;
using Scripts.Gameplay.Fractions;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityFraction : Abillity
    {
        private Fraction fraction;


        [Server]
        public void SetFraction(Fraction fraction) => this.fraction = fraction;

        public Fraction GetFraction() => fraction;

        public bool HasFraction() => fraction != null;
    }
}
