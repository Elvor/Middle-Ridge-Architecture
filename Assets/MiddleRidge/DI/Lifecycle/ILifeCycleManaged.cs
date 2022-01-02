namespace MiddleRidge.DI.Lifecycle
{
    public interface ILifeCycleManaged
    {
        void OnCreate();

        void OnStart();

        void OnDestroy();
    }
}