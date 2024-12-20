using Scripts.Gameplay.Abillities;

namespace Scripts.Gameplay.Visuals
{
    public abstract class VisualNickname : Visual<AbillityNickname>
    {
        protected virtual void OnEnable()
        {
            if (Abillity != null)
                Abillity.OnChangeNickname += SetNickname;

            SetNickname(Abillity.Nickname);
        }

        protected virtual void OnDisable()
        {
            if (Abillity != null)
                Abillity.OnChangeNickname -= SetNickname;
        }


        protected abstract void SetNickname(string nickname);
    }
}
