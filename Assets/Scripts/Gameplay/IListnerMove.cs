using UnityEngine;

namespace Scripts.Gameplay.Listners
{
    interface IListnerMove
    {
        void Move(Vector3 vector);
    }

    interface IListnerFire
    {
        void StartFire();
        void StopFire();

        void CancelFire();
    }
}
