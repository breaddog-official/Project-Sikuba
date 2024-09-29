using UnityEngine;

/// <summary>
/// Class for managing a game session (player spawn, fractions)
/// </summary>
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;
    }
}
