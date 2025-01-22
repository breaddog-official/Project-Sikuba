using NaughtyAttributes;
using UnityEngine;
using System;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerLight : ColorHandler
    {
        [SerializeField] private bool overrideLight;
        [ShowIf(nameof(overrideLight))]
        [SerializeField] private Light overridedLight;
        private Light cachedLight;


        public Light Light => overrideLight ? overridedLight : cachedLight ??= GetComponent<Light>();



        public override void SetColor(Color color)
        {
            if (Light == null)
                throw new ArgumentNullException(nameof(Light));

            Light.color = color;
        }

        public override Color GetColor()
        {
            if (Light == null)
                throw new ArgumentNullException(nameof(Light));

            return Light.color;
        }
    }
}