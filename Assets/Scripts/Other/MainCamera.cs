using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
