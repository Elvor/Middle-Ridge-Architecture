using System;

namespace MiddleRidge.DI.Impl
{
    public class ModuleContainerBuildException : Exception
    {
        public ModuleContainerBuildException(string message) : base(message)
        {
            
        }
    }
}