using System;

namespace MiddleRidge.DI.Lifecycle
{
    public interface ISwitchable
    {
        bool IsActive { get; set; }
        
        bool Switch();
        
        IObservable<bool> OnChangeState();
    }
}