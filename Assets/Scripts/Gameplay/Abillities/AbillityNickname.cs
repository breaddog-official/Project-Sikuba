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

        [field: SyncVar(hook = nameof(ChangeNicknameCallback))]
        public string Nickname { get; protected set; }

        public event Action<string> OnChangeNickname;


        public override bool Initialize()
        {
            if (isServer)
                SetNicknameServer(GetDefaultNickname());

            return base.Initialize();
        }

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
            // We will repeat all checks because client with cheats can skip them
            string handledNickname = HandleNick(nickname);

            if (!ValidNick(handledNickname))
                return;

            Nickname = nickname;
        }



        public virtual bool ValidNick(string nick)
        {
            int characters = nick.ToCharArray().Length;
            if (characters < MinSymbols || characters > MaxSymbols)
                return false;

            return true;
        }

        public virtual string HandleNick(string nick)
        {
            if (DisallowAllWhiteSpace)
                return nick.Replace(" ", "_");

            return nick;
        }


        private void ChangeNicknameCallback(string oldNick, string newNick) => OnChangeNickname?.Invoke(newNick);


        protected virtual string GetDefaultNickname()
        {
            return $"Player_{UnityEngine.Random.Range(1000, 9999)}";
        }
    }
}
