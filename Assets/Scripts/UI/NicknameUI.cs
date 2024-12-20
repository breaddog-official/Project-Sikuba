using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class NicknameUI : MonoBehaviour
    {
        public void SetNickname(string nick)
        {
            if (NetworkClient.localPlayer.TryGetComponent<Entity>(out var entity) && entity.TryFindAbillity<AbillityNickname>(out var nickname))
            {
                nickname.SendNicknameRequest(nick);
            }
        }
    }
}
