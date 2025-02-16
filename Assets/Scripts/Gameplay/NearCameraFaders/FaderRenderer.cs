using Scripts.Gameplay.ColorHandlers;
using UnityEngine;
using NaughtyAttributes;

namespace Scripts.Gameplay.CameraManagement
{
    public class FaderRenderer : Fader
    {
        [SerializeField] protected ColorHandlerRenderer colorHandlerRenderer;
        [Space]
        [SerializeField] protected bool haveMultipleShaders;
        [Range(0f, 1f)]
        [ShowIf(nameof(haveMultipleShaders))]
        [SerializeField] protected float opaqueLevel = 0.97f;
        [ShowIf(nameof(haveMultipleShaders))]
        [SerializeField] protected bool opaqueIsShared;
        [ShowIf(EConditionOperator.And, nameof(haveMultipleShaders), nameof(NonOpaqueIsShared))]
        [SerializeField] protected Shader opaqueShader;
        [ShowIf(nameof(haveMultipleShaders))]
        [SerializeField] protected Shader transparentShader;

        protected Material cachedMaterial;
        protected Material sharedMaterial;


        protected override float CurrentAlpha 
        {
            get => colorHandlerRenderer.GetColor().a;
            set
            {
                Color color = new(colorHandlerRenderer.Color.r, colorHandlerRenderer.Color.g, colorHandlerRenderer.Color.b, value);

                if (sharedMaterial == null)
                    sharedMaterial = colorHandlerRenderer.Renderer.sharedMaterial;

                if (haveMultipleShaders)
                {
                    if (value < opaqueLevel)
                    {
                        // Transparent
                        cachedMaterial.shader = transparentShader;
                    }
                    else if (CurrentAlpha < opaqueLevel)
                    {
                        // Opaque
                        if (false)//opaqueIsShared)
                        {
                            //Destroy(colorHandlerRenderer.Renderer.material);
                            //colorHandlerRenderer.Renderer.sharedMaterial = sharedMaterial;
                            //colorHandlerRenderer.Renderer.material = sharedMaterial;
                        } 
                        else
                            cachedMaterial.shader = opaqueShader;

                        return;
                    }
                }
                
                if (cachedMaterial == null)
                {
                    cachedMaterial = colorHandlerRenderer.SetColorCopyMaterial(color);
                }
                else
                {
                    cachedMaterial.color = color;
                }
            }
        }

        private void OnDestroy()
        {
            if (cachedMaterial != null)
                Destroy(cachedMaterial);
        }


        protected bool NonOpaqueIsShared => !opaqueIsShared;
    }
}