using NaughtyAttributes;
using Scripts.Gameplay.Entities;
using Scripts.Gameplay.Fractions;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityColor : Abillity
    {
        [SerializeField] private RendererFractionColor[] renderers;

        protected AbillityDataFraction dataFraction;


        public override bool Initialize(Entity entity)
        {
            if (entity.TryFindAbillity(out dataFraction))
            {
                dataFraction.OnFractionChange += OnFractionChange;
                OnFractionChange(dataFraction.Get());
            }


            return base.Initialize(entity);
        }

        protected virtual void OnFractionChange(Fraction fraction)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].CalculateFirstColor();
                renderers[i].SetColor(fraction != null ? fraction.GetColor(renderers[i].fractionColor) : renderers[i].firstColor.Value);
            }
        }


        [Serializable]
        private class RendererFractionColor
        {
            enum RendererType
            {
                Renderer,
                Text
            }
            [SerializeField] private RendererType renderType;
            [ShowIf(nameof(renderType), RendererType.Renderer)]
            [SerializeField] private Renderer renderer;
            [ShowIf(nameof(renderType), RendererType.Text)]
            [SerializeField] private TMP_Text text;

            public FractionColor fractionColor;

            public Color? firstColor;



            public void CalculateFirstColor()
            {
                firstColor ??= GetColor();
            }

            public void SetColor(Color color)
            {
                switch (renderType)
                {
                    case RendererType.Renderer:
                        renderer.material.color = color;
                        break;

                    case RendererType.Text:
                        text.color = color;
                        break;
                }
            }

            public Color GetColor()
            {
                return renderType switch
                {
                    RendererType.Renderer => renderer.material.color,
                    RendererType.Text => text.color,
                    _ => Color.magenta
                };
            }
        }
    }
}
