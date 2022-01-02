using System;

namespace MiddleRidge.MVP
{
    public class ScreenStateException : Exception
    {
        public ScreenStateException(Type type, bool state) : base(
            $"{type.Name} is not in valid state: {GetStateName(state)}") {}

        private static String GetStateName(bool state)
        {
            return state ? "active" : "inactive";
        }
    }
}