using System;
using MiddleRidge.DI.Lifecycle;
using UniRx;
using UnityEngine;

namespace MiddleRidge.Test
{
    [TestComponent]
    public class TestScreenManagerEventSource : BaseRxObject, IUpdatable, ITestScreenManagerEventSource
    {
        private readonly Subject<Unit> onBackPressed = new Subject<Unit>();
        public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onBackPressed.OnNext(Unit.Default);
            }
        }

        public IObservable<Unit> OnBackPressed()
        {
            return onBackPressed;
        }
    }
}