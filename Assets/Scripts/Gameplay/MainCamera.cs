using Unity.Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [field: SerializeField] 
    public CinemachineCamera CameraCinemachine { get; private set; }
    public CinemachineFollow CameraFollow { get; private set; }
    public Camera Camera { get; private set; }

    public static MainCamera Instance { get; private set; }

    public Transform CachedTranform { get; private set; }


    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
        CameraFollow = CameraCinemachine.GetComponent<CinemachineFollow>();
        CachedTranform = transform;
    }
}
