using MiddleRidge.DI.Attribute;
using UnityEngine;

namespace MiddleRidge.Test.Menu
{
    [DependencyProvider(typeof(TestComponent))]
    public class ViewProvider
    {
        [Provides]
        public ITestMenuView TestMenuView([InjectName("canvas")] GameObject canvas)
        {
            var panelPrefab = Resources.Load<GameObject>(@"Panel");
            var panel = Object.Instantiate(panelPrefab, canvas.transform);
            return panel.GetComponent<ITestMenuView>();
        }

        [Provides("canvas")]
        public GameObject GetCanvas()
        {
            return GameObject.Find("Canvas");
        }
    }
}