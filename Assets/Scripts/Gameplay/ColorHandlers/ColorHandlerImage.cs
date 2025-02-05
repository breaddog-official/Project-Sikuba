using NaughtyAttributes;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerImage : ColorHandler
    {
        [SerializeField] private bool overrideImage;
        [ShowIf(nameof(overrideImage))]
        [SerializeField] private Image overridedImage;
        private Image cachedImage;


        public Image Image => overrideImage ? overridedImage : cachedImage ??= GetComponent<Image>();



        public override void SetColor(Color color)
        {
            if (Image == null)
                throw new ArgumentNullException(nameof(Text));

            Image.color = color;
        }

        public override Color GetColor()
        {
            if (Image == null)
                throw new ArgumentNullException(nameof(Text));

            return Image.color;
        }
    }
}