using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Extensions;
using System.Threading;
using UnityEngine;
using static UnityEditor.Progress;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityItemSocketFloating : AbillityItemSocket
    {
        [SerializeField] protected Rigidbody floatingArms;
        [SerializeField] protected float dropForce;
        [Space]
        [SerializeField] protected float delayBetweenEquips;

        protected bool canEquipByDelay = true;

        protected CancellationTokenSource equipCancellationToken;

        [Server]
        public override void EquipItem(Item item)
        {
            // Checks
            if (EquippedItem == item || !canEquipByDelay)
                return;


            // Base call
            base.EquipItem(item);

            // Realization
            if (item.TryGetComponent(out Joint joint))
            {
                if (joint is FixedJoint)
                {
                    joint.transform.SetPositionAndRotation(floatingArms.transform.position, floatingArms.transform.rotation);
                }

                joint.connectedBody = floatingArms;
            }

            EquipTimer().Forget();
        }

        [Server]
        public override void DropItem()
        {
            // Checks
            if (EquippedItem == null)
                return;

            // Base call
            base.DropItem();

            // Realization
            if (LastEquippedItem.TryGetComponent(out Joint joint))
            {
                joint.connectedBody = null;
            }

            if (LastEquippedItem.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(floatingArms.transform.forward * dropForce, ForceMode.Impulse);
            }
        }

        private async UniTaskVoid EquipTimer()
        {
            equipCancellationToken.RenewToken();


            canEquipByDelay = false;

            await UniTask.Delay(delayBetweenEquips.ConvertSecondsToMiliseconds(), cancellationToken: equipCancellationToken.Token);

            canEquipByDelay = true;
        }



        public override void OnStopServer()
        {
            equipCancellationToken.ResetToken();

            DropItem();
        }
    }
}
