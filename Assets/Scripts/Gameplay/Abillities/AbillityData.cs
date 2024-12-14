using Mirror;

namespace Scripts.Gameplay.Abillities
{
    public abstract class AbillityData<T> : Abillity
    {
        /// <summary>
        /// Often, this value will need to be synchronized, so if this is necessary, mark it with the <see cref="SyncVarAttribute"/>.
        /// </summary>
        protected abstract T Value { get; set; }



        /// <summary>
        /// Sets value (only on server)
        /// </summary>
        [Server]
        public virtual void Set(T value) => Value = value;

        /// <summary>
        /// Resets value to null (only on server)
        /// </summary>
        [Server]
        public virtual void Void() => Value = default;

        /// <summary>
        /// Gets the value
        /// </summary>
        public virtual T Get() => Value;

        /// <summary>
        /// Checks for value is not null
        /// </summary>
        public virtual bool Has() => Value != null;
    }
}