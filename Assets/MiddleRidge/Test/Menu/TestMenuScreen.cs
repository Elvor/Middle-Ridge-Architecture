using System;
using MiddleRidge.DI.Attribute;
using MiddleRidge.MVP;

namespace MiddleRidge.Test.Menu
{
    [TestComponent]
    public class TestMenuScreen : BaseScreen
    {
        private TestMenuPresenter presenter;
        
        [InjectionConstructor]
        public TestMenuScreen(TestMenuPresenter presenter) : base(presenter)
        {
            this.presenter = presenter;
        }
        
        public IObservable<ITestMenuView.UserChoice> OnMenuUserChoice()
        {
            return presenter.OnMenuUserChoice();
        }
    }
}