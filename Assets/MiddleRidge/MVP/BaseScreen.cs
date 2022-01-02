using System;
using System.Collections.Generic;
using MiddleRidge.DI.Lifecycle;
using UniRx;

namespace MiddleRidge.MVP
{
    public abstract class BaseScreen : ISwitchable, ILifeCycleManaged
    {
        private readonly IReadOnlyCollection<IInteractor> interactors;
        private readonly Subject<bool> onStateChangeSubject = new Subject<bool>();


        private bool isActive;

        protected BaseScreen(params IInteractor[] interactors)
        {
            this.interactors = interactors;
        }


        public void OnCreate()
        {
            foreach (var interactor in interactors)
                if (interactor.IsActive)
                    interactor.Switch();
        }

        public void OnStart()
        {
            //do nothing
        }

        public void OnDestroy()
        {
            //do nothing
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                onStateChangeSubject.OnNext(value);
            }
        }

        public bool Switch()
        {
            IsActive = !IsActive;
            foreach (var interactor in interactors)
            {
                var state = interactor.Switch();
                if (IsActive != state) throw new ScreenStateException(interactor.GetType(), state);
            }

            return IsActive;
        }

        public IObservable<bool> OnChangeState()
        {
            return onStateChangeSubject;
        }
    }
}