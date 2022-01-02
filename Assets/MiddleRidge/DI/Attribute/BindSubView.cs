using System;

namespace MiddleRidge.DI.Attribute
{
    public class BindSubView : System.Attribute
    {
        public BindSubView(string subViewPath)
        {
            SubViewPath = subViewPath;
        }

        public String SubViewPath { get; }
    }
}