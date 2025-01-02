using System;
using NaughtyAttributes;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    [SerializeField] private bool overrideDuration;
    [ShowIf(nameof(overrideDuration))]
    [SerializeField] private float duration;

    private void Awake()
    {
        if (overrideDuration)
        {
            Destroy(gameObject, duration);
        }

        else if (TryGetComponent(out ParticleSystem particleSystem))
        {
            Destroy(gameObject, particleSystem.main.duration);
        }

        else
            throw new ArgumentNullException("particleSystem");
    }
}
