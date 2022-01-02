using System;
using System.Reflection;

namespace MiddleRidge.DI.Impl
{
    public class InjectionException : Exception
    {
        public InjectionException(FieldInfo info) 
            : base($"No dependency for {info.Name} in {info.DeclaringType?.Name}") {}
    }
}