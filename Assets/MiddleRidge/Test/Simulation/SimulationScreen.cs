using MiddleRidge.DI.Attribute;
using MiddleRidge.MVP;

namespace MiddleRidge.Test.Simulation
{
    [TestComponent]
    public class SimulationScreen : BaseScreen
    {
        [InjectionConstructor]
        public SimulationScreen(SimulationInteractor simulationInteractor) : base(simulationInteractor)
        {
        }
    }
}