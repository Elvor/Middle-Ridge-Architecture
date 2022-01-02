using System;
using MiddleRidge.DI.Lifecycle;
using UniRx;

namespace MiddleRidge.MVP
{
    public abstract class ProxyStateInteractor : BaseRxObject, IInteractor
    {
        private readonly Subject<bool> proxyStateObservable = new Subject<bool>();

        public override void OnCreate()
        {
            if (GetHostObject().IsActive)
            {
                OnActivate();
            }
            Subscribe(GetHostObject().OnChangeState(), isActive =>
            {
                if (isActive)
                {
                    proxyStateObservable.OnNext(true);
                    OnActivate();
                }
                else
                {
                    OnDeactivate();
                    proxyStateObservable.OnNext(false);
                }

            });
        }

        protected virtual void OnActivate()
        {
            
        }

        protected virtual void OnDeactivate()
        {
            
        }

        protected abstract IVisibilitySwitchableView GetHostObject();

        public bool IsActive
        {
            get => GetHostObject().IsActive;
            set => GetHostObject().IsActive = value;
        }

        public bool Switch()
        {
            return GetHostObject().Switch();
        }

        public IObservable<bool> OnChangeState()
        {
            return proxyStateObservable;
        }
    }
}