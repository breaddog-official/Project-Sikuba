namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityCollisioner : Abillity
    {
        public abstract bool InAir();


        public abstract bool OnGround();
    }
}