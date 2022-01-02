using System;

namespace MiddleRidge.DI.Impl
{
    public class DependencySearchException : Exception
    {
        public DependencySearchException(string message) : base(message)
        {
        }
    }
}