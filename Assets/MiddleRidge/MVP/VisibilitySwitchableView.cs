using System;
using MiddleRidge.DI;
using UniRx;

namespace MiddleRidge.MVP
{
    public class VisibilitySwitchableView : UnityView, IVisibilitySwitchableView
    {
        private readonly Subject<bool> stateChangeSubject = new Subject<bool>();

        public IObservable<bool> OnChangeState()
        {
            return stateChangeSubject;
        }

        public bool IsActive
        {
            get => gameObject.activeSelf;
            set
            {
                if (value == gameObject.activeSelf)
                {
                    return;
                }
                Switch();
            }
        }

        public bool Switch()
        {
            gameObject.SetActive(!IsActive);
            var isActive = gameObject.activeSelf;
            stateChangeSubject.OnNext(isActive);
            return isActive;
        }
    }
}