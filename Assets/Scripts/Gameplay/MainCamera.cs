using Scripts.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Gameplay.CameraManagement
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] protected CameraEffect[] cameraEffects;

        [field: SerializeField]
        public CinemachineCamera CinemachineCamera { get; private set; }
        public CinemachineFollow CinemachineFollow { get; private set; }
        public Camera Camera { get; private set; }

        public static MainCamera Instance { get; private set; }

        public Transform CachedTranform { get; private set; }


        private void Awake()
        {
            Instance = this;
            Camera = GetComponent<Camera>();
            CinemachineFollow = CinemachineCamera.GetComponent<CinemachineFollow>();
            CachedTranform = transform;
        }

        public T GetCameraEffect<T>() where T : CameraEffect
        {
            return cameraEffects.FindByType<T>();
        }
    }
}
