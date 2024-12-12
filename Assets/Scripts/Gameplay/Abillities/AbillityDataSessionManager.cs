using Mirror;
using Scripts.SessionManagers;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityDataSessionManager : AbillityData<SessionManager>
    {
        [field: SyncVar]
        protected override SessionManager Value { get; set; }
    }
}
