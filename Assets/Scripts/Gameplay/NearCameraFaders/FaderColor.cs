using Scripts.Gameplay.ColorHandlers;
using UnityEngine;

namespace Scripts.Gameplay.CameraManagement
{
    public class FaderColor : Fader
    {
        [field: SerializeField, Space]
        protected virtual ColorHandler ColorHandler { get; set; }

        protected override float CurrentAlpha
        {
            get => ColorHandler.GetColor().a;
            set => ColorHandler.SetColor(new(ColorHandler.Color.r, ColorHandler.Color.g, ColorHandler.Color.b, value));
        }
    }
}