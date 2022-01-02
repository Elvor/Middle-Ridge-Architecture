using System;

namespace MiddleRidge.DI.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectName : System.Attribute
    {
        public string Name { get; }
        
        public InjectName(string name)
        {
            Name = name;
        }
    }
}