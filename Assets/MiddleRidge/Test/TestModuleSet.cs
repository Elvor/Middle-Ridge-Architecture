using MiddleRidge.DI;
using MiddleRidge.DI.Attribute;

namespace MiddleRidge.Test
{
    public class TestModuleSet : AbstractModuleSet
    {
        protected override Component[] GetModuleAttributes()
        {
            return new Component[] {new TestComponent()};
        }
    }
}