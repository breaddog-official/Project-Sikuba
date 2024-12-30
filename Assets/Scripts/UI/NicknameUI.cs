using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using UnityEngine;
using Scripts.SessionManagers;

namespace Scripts.UI
{
    public class NicknameUI : MonoBehaviour
    {
        [SerializeField] private SessionManager sessionManager;

        private string cachedNickname;



        private void OnEnable()
        {
            sessionManager.OnSpawnPlayer += ApplyNick;
        }

        private void OnDisable()
        {
            sessionManager.OnSpawnPlayer -= ApplyNick;
        }


        public void SetNickname(string nick)
        {
            cachedNickname = nick;

            ApplyNick();
        }

        public void ApplyNick()
        {
            if (!NetworkClient.active || NetworkClient.localPlayer == null)
                return;

            if (NetworkClient.localPlayer.TryGetComponent<Entity>(out var entity) && entity.TryFindAbillity<AbillityNickname>(out var nickname))
            {
                nickname.SendNicknameRequest(cachedNickname);
            }
        }
    }
}
