using NaughtyAttributes;
using System;
using UnityEngine;

namespace Scripts.Settings
{
    [Serializable]
    public class Settings
    {
        #region Enums

        public enum ShadowsResolution
        {
            Low,
            Medium,
            High,
            VeryHigh
        }

        public enum ParticlesMode
        {
            Disable,
            [Tooltip("Without lithing. Low cost shaders")]
            Simple,
            Normal
        }

        public enum ShadersQuality
        {
            [Tooltip("Unlit shaders")]
            Lowest,
            [Tooltip("Mobile Diffuse and VertexLit shaders")]
            Low,
            [Tooltip("Diffuse shaders")]
            Medium,
            [Tooltip("Diffuse and Standart shaders")]
            High,
            [Tooltip("Standart shaders (or Autodesk)")]
            Extreme
        }

        public enum AntiAliasing
        {
            None,
            FXAA,
            SMAA,
            TAA,
            MSAA,
            FSR,
        }

        public enum TexturesResolution
        {
            Low,
            Medium,
            High
        }

        public enum AnisotropicFiltration
        {
            None,
            x2,
            x4,
            x8,
            x16,
        }

        #endregion

        #region Fields

        [Header("Volume")]
        public float soundsVolume;
        public float musicVolume;

        [Header("Video")]
        public FullScreenMode fullsceenMode;
        public Resolution resolution;

        [Header("Graphics")]
        public bool shadowsEnabled;
        [ShowIf(nameof(shadowsEnabled))]
        public ShadowsResolution shadowsResolution = ShadowsResolution.Medium;
        public ParticlesMode particlesMode = ParticlesMode.Normal;
        public ShadersQuality shadersQuality = ShadersQuality.Medium;
        public AnisotropicFiltration anisotropicFiltration = AnisotropicFiltration.x4;
        public AntiAliasing antiAliasing;
        public TexturesResolution texturesResolution;

        [HideInInspector]
        public string debugCode;

        #endregion

        #region Reflections

        public void SetValue(string name, object value)
        {
            typeof(Settings).GetField(name).SetValue(this, value);
        }


        public object GetValue(string name)
        {
            return typeof(Settings).GetField(name).GetValue(this);
        }

        public T GetValue<T>(string name)
        {
            return (T)GetValue(name);
        }

        #endregion
    }
}