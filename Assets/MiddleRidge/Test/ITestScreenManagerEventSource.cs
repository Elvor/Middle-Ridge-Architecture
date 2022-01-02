using System;
using UniRx;

namespace MiddleRidge.Test
{
    public interface ITestScreenManagerEventSource
    {
        IObservable<Unit> OnBackPressed();
    }
}