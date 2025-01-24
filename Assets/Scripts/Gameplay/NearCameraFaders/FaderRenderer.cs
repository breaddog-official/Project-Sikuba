using Scripts.Gameplay.ColorHandlers;
using UnityEngine;
using NaughtyAttributes;

namespace Scripts.Gameplay.NearCamera
{
    public class FaderRenderer : Fader
    {
        [SerializeField] protected ColorHandlerRenderer colorHandlerRenderer;
        [Space]
        [SerializeField] protected bool haveMultipleShaders;
        [Range(0f, 1f)]
        [ShowIf(nameof(haveMultipleShaders))]
        [SerializeField] protected float opaqueLevel = 0.97f;
        //[ShowIf(nameof(haveMultipleShaders))]
        //[SerializeField] protected bool opaqueIsShared;
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
                sharedMaterial ??= colorHandlerRenderer.Renderer.sharedMaterial;
                Color color = new(colorHandlerRenderer.Color.r, colorHandlerRenderer.Color.g, colorHandlerRenderer.Color.b, value);

                if (cachedMaterial == null)
                {
                    cachedMaterial = colorHandlerRenderer.SetColorCopyMaterial(color);
                }
                else
                {
                    cachedMaterial.color = color;
                    colorHandlerRenderer.SetMaterial(cachedMaterial);
                }

                if (haveMultipleShaders)
                {
                    print(CurrentAlpha);
                    if (CurrentAlpha < opaqueLevel)
                    {
                        cachedMaterial.shader = transparentShader;
                    }
                    else
                    {
                        //if (opaqueIsShared)
                        //    colorHandlerRenderer.SetMaterial(sharedMaterial);
                        //else
                            cachedMaterial.shader = opaqueShader;
                    }
                }

                
            }
        }

        private void OnDestroy()
        {
            if (cachedMaterial != null)
                Destroy(cachedMaterial);
        }


        protected bool NonOpaqueIsShared => true;//!opaqueIsShared;
    }
}