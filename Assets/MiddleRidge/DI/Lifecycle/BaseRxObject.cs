using System;

namespace MiddleRidge.DI.Lifecycle
{
    public abstract class BaseRxObject : ILifeCycleManaged
    {
        private SubscriptionManager subscriptionManager = new SubscriptionManager();        

        protected IDisposable Subscribe<T>(IObservable<T> observable, Action<T> onNext)
        {
            return subscriptionManager.Subscribe(observable, onNext);
        }

        protected void Unsubscribe(IDisposable sub)
        {
            subscriptionManager.Unsubscribe(sub);
        }

        protected void ClearSubscriptions()
        {
            subscriptionManager.ClearSubscriptions();
        }
        
        public virtual void OnDestroy()
        {
            subscriptionManager.ClearSubscriptions();
        }



        public virtual void OnCreate()
        {
            
        }

        public virtual void OnStart()
        {
            
        }
    }
}