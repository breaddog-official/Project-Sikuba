using UnityEngine;

namespace Scripts.MonoCacher
{
    public interface IMonoCacheListner
    {
        Behaviour Behaviour { get; }
    }



    public interface IMonoCacheUpdate : IMonoCacheListner
    {
        void UpdateCached();
    }

    public interface IMonoCacheFixedUpdate : IMonoCacheListner
    {
        void FixedUpdateCached();
    }

    public interface IMonoCacheLateUpdate : IMonoCacheListner
    {
        void LateUpdateCached();
    }
}