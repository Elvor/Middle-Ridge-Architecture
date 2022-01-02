using System;

namespace MiddleRidge.DI.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class Component : System.Attribute
    {
        protected Component(int handleOrder = 0, string name = null)
        {
            this.HandleOrder = handleOrder;
            this.Name = name;
        }

        protected Component(string name = null)
        {
            HandleOrder = 0;
            this.Name = name;
        }

        protected Component()
        {
            HandleOrder = 0;
            Name = null;
        }

        public int HandleOrder { get; }

        public string Name { get; }
    }
}