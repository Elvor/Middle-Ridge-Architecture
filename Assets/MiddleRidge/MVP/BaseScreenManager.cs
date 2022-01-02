using System.Collections.Generic;
using MiddleRidge.DI.Lifecycle;

namespace MiddleRidge.MVP
{
    public abstract class BaseScreenManager : BaseRxObject
    {
        private readonly Stack<ISwitchable> screenStack = new Stack<ISwitchable>();

        protected void AddScreen(ISwitchable screen)
        {
            if (!screen.IsActive) screen.Switch();
            if (screenStack.Count > 0) screenStack.Peek().Switch();
            screenStack.Push(screen);
        }

        protected void ReplaceScreen(ISwitchable screen)
        {
            if (!screen.IsActive) screen.Switch();
            screenStack.Pop().Switch();
            screenStack.Push(screen);
        }

        /// <summary>
        ///     Pops screen from stack
        /// </summary>
        /// <returns><code>true</code> if screen is popped, <code>false</code> otherwise (only one screen remains)</returns>
        protected PopResult PopScreen()
        {
            if (screenStack.Count <= 1) return PopResult.LastScreen;
            screenStack.Pop().Switch();
            screenStack.Peek().Switch();
            return PopResult.Ok;
        }

        protected enum PopResult
        {
            Ok,
            ConfirmationRequired,
            LastScreen
        }
    }
}