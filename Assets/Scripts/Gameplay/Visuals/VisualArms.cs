using UnityEngine;
using Scripts.Gameplay.Abillities;
using Scripts.Extensions;
using Mirror;
namespace Scripts.Gameplay.Visuals
{
    public abstract class VisualArms : Visual
    {
        [SerializeField] protected AbillityArms abillityArms;

        protected bool subscribed;


        protected virtual void OnEnable() => Subscribe();
        protected virtual void OnDisable() => Unsubscribe();


        protected virtual void Subscribe()
        {
            if (subscribed || !abillityArms.AvailableAndNotNull())
                return;

            subscribed = true;


            abillityArms.OnEquip += VisualEquip;
            abillityArms.OnThrow += VisualThrow;
        }

        protected virtual void Unsubscribe()
        {
            if (!subscribed || !abillityArms.AvailableAndNotNull())
                return;

            subscribed = false;


            abillityArms.OnEquip -= VisualEquip;
            abillityArms.OnThrow -= VisualThrow;
        }

        /// <summary>
        /// You MUST override this
        /// </summary>
        [ClientRpc] public virtual void VisualEquip() { }
        /// <summary>
        /// You MUST override this
        /// </summary>
        [ClientRpc] public virtual void VisualThrow() { }
    }
}
