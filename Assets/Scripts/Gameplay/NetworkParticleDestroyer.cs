using System;
using NaughtyAttributes;
using UnityEngine;
using Mirror;
using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using System.Threading;

public class NetworkParticleDestroyer : NetworkBehaviour
{
    [SerializeField] private bool overrideDuration;
    [ShowIf(nameof(overrideDuration))]
    [SerializeField] private float duration;

    private CancellationTokenSource cancellationToken;


    public override void OnStartServer()
    {
        if (overrideDuration)
        {
            DestroyTimer(duration).Forget();
        }

        else if (TryGetComponent(out ParticleSystem particleSystem))
        {
            DestroyTimer(particleSystem.main.duration).Forget();
        }

        else
            throw new ArgumentNullException(nameof(particleSystem));
    }

    private async UniTaskVoid DestroyTimer(float delay)
    {
        cancellationToken?.ResetToken();
        cancellationToken ??= new();

        await UniTask.Delay(delay.ConvertSecondsToMiliseconds(), cancellationToken: cancellationToken.Token);

        NetworkServer.Destroy(gameObject);
    }
}