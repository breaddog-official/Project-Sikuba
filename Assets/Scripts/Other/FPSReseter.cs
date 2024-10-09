using NaughtyAttributes;
using UnityEngine;

public class FPSReseter : MonoBehaviour
{
    [SerializeField] private bool resetOnAwake;

    void Awake()
    {
        if (resetOnAwake)
            ResetFPS();
    }

    [Button]
    public void ResetFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 1024;
    }
}
