using UnityEngine;
using Mirror;
using Scripts.Gameplay.Abillities;
using Scripts.Gameplay.Entities;

namespace Scripts.Gameplay.Visuals
{
    /// <summary>
    /// Class for displaying abillities actions
    /// </summary>
    public abstract class Visual<TAbillity> : MonoBehaviour where TAbillity : Abillity
    {
        [field: SerializeField] public TAbillity Abillity { get; protected set; }
    }
}
