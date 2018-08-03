using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamRipper.Interfaces;

namespace StreamRipper.Builders
{
    public class ActionEventHandlerBuilder<T>: BaseBuilder<ActionEventHandlerBuilder<T>, Action<T>>, IActionEventHandlerBuilder<T>
    {
        private Action<T> _action;

        private readonly List<Action<T>> _beforeExecution = new List<Action<T>>();
        
        private readonly List<Action<T>> _afterExecution = new List<Action<T>>();
        
        private readonly List<Func<T, bool>> _filterExecution = new List<Func<T, bool>>();
        
        private bool _async = true;

        /// <summary>
        /// Wrap the action as async task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IActionEventHandlerBuilder<T> WrapAsync() => Run(this, () => _async = true);
        
        /// <summary>
        /// Set the action event handler
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IActionEventHandlerBuilder<T> SetActionHandler(Action<T> action) => Run(this, () => _action = action);

        /// <summary>
        /// Add before execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IActionEventHandlerBuilder<T> AddBeforeExecution(Action<T> action) => Run(this, () => _beforeExecution.Add(action));

        /// <summary>
        /// Add after execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IActionEventHandlerBuilder<T> AddAfterExecution(Action<T> action) => Run(this, () => _afterExecution.Add(action));

        /// <summary>
        /// Add filter execution
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IActionEventHandlerBuilder<T> AddFilterExecution(Func<T, bool> filter) => Run(this, () => _filterExecution.Add(filter));
        
        /// <inheritdoc />
        /// <summary>
        /// Before the build
        /// </summary>
        protected override void BeforeBuild() => _action = _action ?? EmptyAction<T>();

        /// <summary>
        /// Build the complete action
        /// </summary>
        /// <returns></returns>
        public override Action<T> Build() => Run(new Action<T>(x =>
        {
            if (_async)
            {
                Task.Run(() => ActionTask(x));
            }
            else
            {
                ActionTask(x);
            }
        }), BeforeBuild);
        
        /// <summary>
        /// Actiont task
        /// </summary>
        /// <param name="arg"></param>
        private void ActionTask(T arg)
        {
            // Make sure filters passes
            if (_filterExecution.All(filter => filter(arg)))
            {
                _beforeExecution.ForEach(action => action(arg));
                _action(arg);
                _afterExecution.ForEach(action => action(arg));
            }
        }
    }
}