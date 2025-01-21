using UnityEngine;

namespace Scripts.Gameplay.ColorHandlers
{
    /// <summary>
    /// Since normal unity classes (such as renderer, particle system) do not have an interface for accessing color, this class was created.
    /// </summary>
    public abstract class ColorHandler : MonoBehaviour
    {
        public virtual Color Color
        {
            get => GetColor();
            set => SetColor(value);
        }

        public abstract void SetColor(Color color);
        public abstract Color GetColor();
    }
}