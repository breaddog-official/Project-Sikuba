using UnityEngine;

namespace Scripts.Gameplay
{
    public enum BandwidthOptimizationMode
    {
        [Tooltip("Without bandwidth optimizations.")]
        None,

        [Tooltip("Applying bandwidth optimizations, but if client ignores it, we do nothing.")]
        Normal,

        [Tooltip("Applying bandwidth optimizations, but if client ignores it, we do it itself. In this mode we will a little increase the server's performance and a little reduce the accuracy of calculations.")]
        Aggressive
    }
}
