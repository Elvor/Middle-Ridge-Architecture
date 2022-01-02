using System;
using MiddleRidge.DI.Attribute;
using MiddleRidge.MVP;

namespace MiddleRidge.Test.Menu
{
    [TestComponent]
    public class TestMenuPresenter : ProxyStateInteractor
    {
        private ITestMenuView testMenuView;

        [InjectionConstructor]
        public TestMenuPresenter(ITestMenuView testMenuView)
        {
            this.testMenuView = testMenuView;
        }

        protected override IVisibilitySwitchableView GetHostObject()
        {
            return testMenuView;
        }

        public IObservable<ITestMenuView.UserChoice> OnMenuUserChoice()
        {
            return testMenuView.OnActionChosen();
        }
        
    }
}