using System;

namespace StreamRipper.Interfaces
{
    public interface IActionEventHandlerBuilder<T>
    {
        IActionEventHandlerBuilder<T> WrapAsync();

        IActionEventHandlerBuilder<T> SetActionHandler(Action<T> action);

        IActionEventHandlerBuilder<T> AddBeforeExecution(Action<T> action);

        IActionEventHandlerBuilder<T> AddAfterExecution(Action<T> action);

        IActionEventHandlerBuilder<T> AddFilterExecution(Func<T, bool> filter);
        
        Action<T> Build();
    }
}