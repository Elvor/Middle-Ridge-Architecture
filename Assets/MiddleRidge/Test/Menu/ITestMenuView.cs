using System;
using MiddleRidge.MVP;

namespace MiddleRidge.Test.Menu
{
    public interface ITestMenuView : IVisibilitySwitchableView
    {
        IObservable<UserChoice> OnActionChosen();

        public enum UserChoice
        {
            Simulation,
            Exit
        }
    }
}