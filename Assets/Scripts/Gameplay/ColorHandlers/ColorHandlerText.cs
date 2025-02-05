using NaughtyAttributes;
using UnityEngine;
using System;
using TMPro;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerText : ColorHandler
    {
        [SerializeField] private bool overrideText;
        [ShowIf(nameof(overrideText))]
        [SerializeField] private TMP_Text overridedText;
        private TMP_Text cachedText;


        public TMP_Text Text => overrideText ? overridedText : cachedText ??= GetComponent<TMP_Text>();



        public override void SetColor(Color color)
        {
            if (Text == null)
                throw new ArgumentNullException(nameof(Text));

            Text.color = color;
        }

        public override Color GetColor()
        {
            if (Text == null)
                throw new ArgumentNullException(nameof(Text));

            return Text.color;
        }
    }
}