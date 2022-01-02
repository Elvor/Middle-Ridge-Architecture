using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MiddleRidge.DI.Lifecycle
{
    public class SubscriptionManager
    {
        private readonly IList<IDisposable> subscriptions = new List<IDisposable>();
        
        public IDisposable Subscribe<T>(IObservable<T> observable, Action<T> onNext)
        {
            var sub = observable.Subscribe(onNext);
            this.subscriptions.Add(sub);
            return sub;
        }

        public void Unsubscribe(IDisposable sub)
        {
            if (!subscriptions.Remove(sub))
            {
                Debug.LogWarning("Cancelled subscription is from another object");
            }
            sub.Dispose();
        }
        
        public void ClearSubscriptions()
        {
            foreach (var subscription in this.subscriptions)
            {
                subscription.Dispose();
            }
            this.subscriptions.Clear();
        }
    }
}