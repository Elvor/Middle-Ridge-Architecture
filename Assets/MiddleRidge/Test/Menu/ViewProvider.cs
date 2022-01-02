using MiddleRidge.DI.Attribute;
using UnityEngine;

namespace MiddleRidge.Test.Menu
{
    [DependencyProvider(typeof(TestComponent))]
    public class ViewProvider
    {
        [Provides]
        private ITestMenuView TestMenuView()
        {
            return GameObject.Find("Panel").GetComponent<ITestMenuView>();
        }
    }
}