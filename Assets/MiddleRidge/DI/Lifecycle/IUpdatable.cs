namespace MiddleRidge.DI.Lifecycle
{
    public interface IUpdatable : ILifeCycleManaged
    {
        void Update(float deltaTime);
    }
}