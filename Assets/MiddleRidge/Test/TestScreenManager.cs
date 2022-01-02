using MiddleRidge.DI.Attribute;
using MiddleRidge.MVP;
using MiddleRidge.Test.Menu;
using MiddleRidge.Test.Simulation;
using UnityEngine;

namespace MiddleRidge.Test
{
    [TestComponent]
    public class TestScreenManager : BaseScreenManager
    {
        private readonly SimulationScreen simulationScreen;
        private readonly TestMenuScreen testMenuScreen;
        private ITestScreenManagerEventSource testScreenManagerEventSource;

        [InjectionConstructor]
        public TestScreenManager(SimulationScreen simulationScreen, TestMenuScreen testMenuScreen,
            ITestScreenManagerEventSource testScreenManagerEventSource)
        {
            this.simulationScreen = simulationScreen;
            this.testMenuScreen = testMenuScreen;
            this.testScreenManagerEventSource = testScreenManagerEventSource;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Subscribe(testScreenManagerEventSource.OnBackPressed(), unit => GoBack());
            Subscribe(testMenuScreen.OnMenuUserChoice(), choice =>
            {
                switch (choice)
                {
                    case ITestMenuView.UserChoice.Exit:
                        Exit();
                        break;
                    case ITestMenuView.UserChoice.Simulation:
                        OpenSimulation();
                        break;
                }
            });
        }

        public override void OnStart()
        {
            base.OnStart();
            AddScreen(testMenuScreen);
        }

        private void OpenSimulation()
        {
            AddScreen(simulationScreen);
        }

        private void Exit()
        {
            Application.Quit();
        }

        public void GoBack()
        {
            var result = PopScreen();
            if (result == PopResult.LastScreen) Exit();
        }
    }
}