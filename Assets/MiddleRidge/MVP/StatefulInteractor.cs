using System;
using MiddleRidge.DI.Lifecycle;
using UniRx;

namespace MiddleRidge.MVP
{
    public abstract class StatefulInteractor : BaseRxObject, IInteractor
    {
        private readonly Subject<bool> onChangeStateSubject = new Subject<bool>();
        public override void OnCreate()
        {
            
        }

        private bool isActive = false;
        
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (value == isActive)
                {
                    return;
                }
                isActive = value;
                if (value)
                {
                    onChangeStateSubject.OnNext(true);
                    OnActivate();
                }
                else
                {
                    OnDeactivate();
                    onChangeStateSubject.OnNext(false);
                }
            }
        }

        public bool Switch()
        {
            IsActive = !IsActive;
            return IsActive;
        }

        public IObservable<bool> OnChangeState()
        {
            return onChangeStateSubject;
        }

        protected virtual void OnActivate()
        {
            
        }

        protected virtual void OnDeactivate()
        {
            ClearSubscriptions();
        }
    }
}