using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.Gameplay.Fractions;
using Scripts.SessionManagers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI
{
    public class FractionSpawnerUI : MonoBehaviour
    {
        [SerializeField] private Transform parentSpawn;
        [SerializeField] private SelectionFractionUI fractionPrefab;
        [Space]
        [SerializeField] private SessionManagerDeathmatch sessionManager;
        [Space]
        [SerializeField] private UnityEvent<Fraction> onFractionClick;
        [Space]
        [SerializeField] private float updateValuesDelay = 1f;
        [SerializeField] private bool stopUpdateAfterFind;

        private readonly HashSet<SelectionFractionUI> spawnedFractionUI = new();
        private CancellationTokenSource cancellationToken;



        protected virtual void OnEnable()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            FractionsUpdater(cancellationToken.Token).Forget();
        }

        protected virtual void OnDisable()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();
        }

        protected virtual void OnDestroy()
        {
            cancellationToken?.ResetToken();
        }



        private async UniTaskVoid FractionsUpdater(CancellationToken token = default)
        {
            HashSet<Fraction> fractionsUi = new();
            HashSet<Fraction> fractions = new();

            while (true)
            {
                fractionsUi.Clear();
                fractions.Clear();

                if (spawnedFractionUI.Count > 0)
                    fractionsUi.AddRange(spawnedFractionUI.Select(f => f.Fraction));

                if (sessionManager.GetFractions().Count > 0)
                    fractions.AddRange(sessionManager.GetFractions().Select(p => p.Key.FindByID<Fraction>()));

                foreach (var fraction in fractions)
                {
                    if (fraction == null)
                        continue;

                    if (!fractionsUi.Contains(fraction))
                    {
                        var fractionUi = Instantiate(fractionPrefab, parentSpawn);
                        fractionsUi.Add(fraction);
                        spawnedFractionUI.Add(fractionUi);

                        fractionUi.Initialize(fraction, onFractionClick.Invoke);
                    }
                }

                if (stopUpdateAfterFind && fractionsUi.Count > 0)
                    break;

                await UniTask.Delay(updateValuesDelay.ConvertSecondsToMiliseconds(), cancellationToken: token);
            }
        }
    }
}