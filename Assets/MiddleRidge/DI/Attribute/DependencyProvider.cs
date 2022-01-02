using System;

namespace MiddleRidge.DI.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyProvider : System.Attribute
    {
        public DependencyProvider(Type componentType)
        {
            ComponentType = componentType;
        }

        public Type ComponentType { get; }
    }
}