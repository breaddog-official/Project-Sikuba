namespace Scripts 
{
    public interface IInitializable
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
