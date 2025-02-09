using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using System.Threading;
using UnityEngine;

namespace Scripts.Gameplay.CameraManagement
{
    public abstract class Fader : MonoBehaviour
    {
        [SerializeField] protected float alphaDefault = 1f;
        [SerializeField] protected float alphaFade = 0.6f;
        [Space]
        [SerializeField] protected float showSpeed = 1f;
        [SerializeField] protected float fadeSpeed = 1f;

        protected abstract float CurrentAlpha { get; set; }

        protected CancellationTokenSource cancellationToken;



        public void Show()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            InterpolateTo(alphaDefault, cancellationToken.Token).Forget();
        }

        public void Fade()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            InterpolateTo(alphaFade, cancellationToken.Token).Forget();
        }



        private async UniTaskVoid InterpolateTo(float alpha, CancellationToken token = default)
        {
            while (CurrentAlpha < alpha)
            {
                await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                CurrentAlpha += Time.deltaTime * showSpeed;
            }

            while (CurrentAlpha > alpha)
            {
                await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                CurrentAlpha -= Time.deltaTime * fadeSpeed;
            }
        }
    }
}