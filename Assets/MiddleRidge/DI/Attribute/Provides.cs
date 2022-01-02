using System;

namespace MiddleRidge.DI.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Provides : System.Attribute
    {
        public Provides(int handleOrder = 0, string name = null)
        {
            HandleOrder = handleOrder;
            Name = name;
        }

        public Provides(string name)
        {
            Name = name;
            HandleOrder = 0;
        }

        public Provides()
        {
            Name = null;
            HandleOrder = 0;
        }

        public int HandleOrder { get; }
        public string Name { get; }
    }
}