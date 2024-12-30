using UnityEngine;
using Mirror;
using System;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityNickname : Abillity
    {
        [field: SerializeField, Min(0)] public ushort MinSymbols { get; protected set; } = 1;
        [field: SerializeField, Min(0)] public ushort MaxSymbols { get; protected set; } = 12;
        [field: SerializeField] public bool DisallowAllWhiteSpace { get; protected set; }
        [field: Space]
        [field: SerializeField] public bool CanChangeNicknameMultipleTimes { get; protected set; }

        [field: SyncVar(hook = nameof(ChangeNicknameCallback))]
        public string Nickname { get; protected set; }

        public event Action<string> OnChangeNickname;



        [Client]
        public virtual void SendNicknameRequest(string nickname)
        {
            string handledNickname = HandleNick(nickname);

            if (!ValidNick(handledNickname))
                return;

            SetNicknameServer(handledNickname);
        }

        [Command]
        public virtual void SetNicknameServer(string nickname)
        {
            if (!CanChangeNicknameMultipleTimes && !string.IsNullOrWhiteSpace(Nickname))
                return;

            // We will repeat all checks because client with cheats can skip them
            string handledNickname = HandleNick(nickname);

            if (!ValidNick(handledNickname))
                nickname = GetDefaultNickname();

            Nickname = nickname;
        }



        public virtual bool ValidNick(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick))
                return false;

            int characters = nick.ToCharArray().Length;
            if (characters < MinSymbols || characters > MaxSymbols)
                return false;

            return true;
        }

        public virtual string HandleNick(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick))
                return string.Empty;

            if (DisallowAllWhiteSpace)
                return nick.Replace(" ", "_");

            return nick;
        }


        private void ChangeNicknameCallback(string oldNick, string newNick) => OnChangeNickname?.Invoke(newNick);


        public static string GetDefaultNickname()
        {
            return $"Player_{UnityEngine.Random.Range(1000, 9999)}";
        }
    }
}
