using Cysharp.Threading.Tasks;
using Scripts.Gameplay.ColorHandlers;
using Scripts.Extensions;
using System.Threading;
using UnityEngine;

namespace Scripts.Gameplay.NearCamera
{
    public class AlphaFader : MonoBehaviour
    {
        [SerializeField] protected float alphaDefault = 1f;
        [SerializeField] protected float alphaFade = 0.6f;
        [Space]
        [SerializeField] protected float showSpeed = 1f;
        [SerializeField] protected float fadeSpeed = 1f;

        private ColorHandler colorHandler;
        private CancellationTokenSource cancellationToken;

        private float CurrentAlpha
        {
            get => colorHandler.GetColor().a;
            set => colorHandler.SetColor(new(colorHandler.Color.r, colorHandler.Color.g, colorHandler.Color.b, value));
        }


        private void Awake()
        {
            colorHandler = GetComponent<ColorHandler>();
        }


        public void Show()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            InterpolateTo(alphaDefault).Forget();
        }

        public void Fade()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            InterpolateTo(alphaFade).Forget();
        }



        private async UniTaskVoid InterpolateTo(float alpha)
        {
            while (CurrentAlpha < alpha)
            {
                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken.Token);
                CurrentAlpha += Time.deltaTime * showSpeed;
            }

            while (CurrentAlpha > alpha)
            {
                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken.Token);
                CurrentAlpha -= Time.deltaTime * fadeSpeed;
            }
        }
    }
}