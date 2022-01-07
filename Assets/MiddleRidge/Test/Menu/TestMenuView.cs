using System;
using MiddleRidge.DI.Attribute;
using MiddleRidge.MVP;
using UniRx;
using UnityEngine.UI;

namespace MiddleRidge.Test.Menu
{
    [BindViews]
    public class TestMenuView : VisibilitySwitchableView, ITestMenuView
    {
        [BindSubView("Exit")] private Button exitBtn;

        // private Subject<ITestMenuView.UserChoice> onActionChosen;
        [BindSubView("Simulation")] private Button simulationBtn;

        public IObservable<ITestMenuView.UserChoice> OnActionChosen()
        {
            return simulationBtn.OnClickAsObservable()
                .Select(unit => ITestMenuView.UserChoice.Simulation).Merge(exitBtn.OnClickAsObservable()
                    .Select(unit => ITestMenuView.UserChoice.Exit));
        }
    }
}