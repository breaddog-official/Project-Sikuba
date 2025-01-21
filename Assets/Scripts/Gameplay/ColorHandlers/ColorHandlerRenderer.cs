using NaughtyAttributes;
using UnityEngine;
using System;

namespace Scripts.Gameplay.ColorHandlers
{
    public class ColorHandlerRenderer : ColorHandler
    {
        [SerializeField] private bool overrideRenderer;
        [ShowIf(nameof(overrideRenderer))]
        [SerializeField] private Renderer overridedRenderer;
        private Renderer cachedRenderer;


        public Renderer Renderer => overrideRenderer ? overridedRenderer : cachedRenderer ??= GetComponent<Renderer>();



        /// <summary>
        /// Sets color to shared material. Not recomended
        /// </summary>
        public override void SetColor(Color color)
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            Renderer.sharedMaterial.color = color;
        }

        /// <summary>
        /// Sets color to copied material. Dynamic instancing will be disabled
        /// </summary>
        public Material SetColorCopyMaterial(Color color)
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            var material = Renderer.material;
            material.color = color;

            return material;
        }

        /// <summary>
        /// Sets new material ro renderer
        /// </summary>
        public void SetMaterial(Material material)
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            Renderer.material = material;
        }

        public Material GetMaterial()
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            return Renderer.material;
        }

        public override Color GetColor()
        {
            if (Renderer == null)
                throw new ArgumentNullException(nameof(Renderer));

            return Renderer.material.color;
        }
    }
}