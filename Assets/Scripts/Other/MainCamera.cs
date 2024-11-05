using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    public Camera Camera { get; private set; }

    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
    }
}
