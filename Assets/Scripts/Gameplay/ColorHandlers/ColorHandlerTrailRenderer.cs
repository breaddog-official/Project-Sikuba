using NaughtyAttributes;
using UnityEngine;
using System;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerTrailRenderer : ColorHandler
    {
        [SerializeField] private bool overrideRenderer;
        [ShowIf(nameof(overrideRenderer))]
        [SerializeField] private TrailRenderer overridedRenderer;
        private TrailRenderer cachedRenderer;


        public TrailRenderer Renderer => overrideRenderer ? overridedRenderer : cachedRenderer ??= GetComponent<TrailRenderer>();




        public override void SetColor(Color color)
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            for (int i = 0; i < Renderer.colorGradient.colorKeys.Length; i++)
            {
                Renderer.colorGradient.colorKeys[i].color = color;
            }
        }

        public override Color GetColor()
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            return AverageColor(Renderer.colorGradient.colorKeys);
        }



        protected virtual Color AverageColor(GradientColorKey[] keys)
        {
            Vector4 rgba = new();

            foreach(var key in keys)
            {
                rgba.x += key.color.r;
                rgba.y += key.color.g;
                rgba.z += key.color.b;
                rgba.w += key.color.a;
            }

            return rgba / keys.Length;
        }
    }
}