using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamRipper.Builders
{
    public class ActionEventHandlerBuilder<T>: BaseBuilder<ActionEventHandlerBuilder<T>, Action<T>>
    {
        private Action<T> _action;

        private readonly List<Action<T>> _beforeExecution = new List<Action<T>>();
        
        private readonly List<Action<T>> _afterExecution = new List<Action<T>>();
        
        private bool _async;

        /// <summary>
        /// Wrap the action as async task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ActionEventHandlerBuilder<T> WrapAsync() => Run(this, () => _async = true);
        
        /// <summary>
        /// Set the action event handler
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionEventHandlerBuilder<T> SetActionHandler(Action<T> action) => Run(this, () => _action = action);

        /// <summary>
        /// Add before execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionEventHandlerBuilder<T> AddBeforeExecution(Action<T> action) => Run(this, () => _beforeExecution.Add(action));

        /// <summary>
        /// Add after execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionEventHandlerBuilder<T> AddAfterExecution(Action<T> action) => Run(this, () => _afterExecution.Add(action));
        
        /// <inheritdoc />
        /// <summary>
        /// Before the build
        /// </summary>
        protected override void BeforeBuild() => _action = _action ?? EmptyAction<T>();

        /// <inheritdoc />
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
            _beforeExecution.ForEach(action => action(arg));
            _action(arg);
            _afterExecution.ForEach(action => action(arg));
        }
    }
}