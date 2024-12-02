using Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [field: SerializeField] 
    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    public Camera Camera { get; private set; }

    public static MainCamera Instance { get; private set; }

    

    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
    }
}
