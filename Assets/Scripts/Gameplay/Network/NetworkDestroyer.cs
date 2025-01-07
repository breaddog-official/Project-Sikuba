using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using UnityEngine;

namespace Mirror
{
    public class NetworkDestroyer : MonoBehaviour
    {
        [SerializeField] private uint checkEveryFrames = 3;
        [SerializeField] private float destroyUnspawnedAfter = 1f;

        private NetworkIdentity identity;



        private void Start()
        {
            identity = GetComponent<NetworkIdentity>();

            CheckCycle().Forget();
        }

        protected virtual void Check()
        {
            if (IsSpawned() == false)
            {
                DestroyCycle().Forget();
            }
        }

        protected virtual bool IsSpawned()
        {
            return NetworkClient.spawned.ContainsValue(identity);
        }



        private async UniTaskVoid CheckCycle()
        {
            while (this != null)
            {
                Check();

                for (int i = 0; i < checkEveryFrames; i++)
                {
                    await UniTask.NextFrame();
                }
            }
        }

        private async UniTaskVoid DestroyCycle()
        {
            await UniTask.Delay(destroyUnspawnedAfter.ConvertSecondsToMiliseconds());

            if (this != null && IsSpawned() == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
