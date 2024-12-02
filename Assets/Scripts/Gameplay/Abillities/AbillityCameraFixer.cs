using UnityEngine;

namespace Scripts.Gameplay.Abillities
{
    public class AbillityCameraFixer : Abillity
    {
        [SerializeField] private Transform target;

        protected override void OnEnable()
        {
            EnableFollow(true);
        }

        protected override void OnDisable()
        {
            EnableFollow(false);
        }


        protected virtual void EnableFollow(bool state)
        {
            if (MainCamera.Instance != null && MainCamera.Instance.VirtualCamera != null)
            {
                MainCamera.Instance.VirtualCamera.Follow = state ? target : null;
            }
        }
    }
}