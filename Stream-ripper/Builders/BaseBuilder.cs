using System;
using System.Threading.Tasks;
using StreamRipper.Extensions;

namespace StreamRipper.Builders
{
    /// <summary>
    /// Base builder, needed so that fields of mappers are re-created during each mapping process
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public abstract class BaseBuilder<TBuilder, TDestination> where TBuilder : new()
    {
        /// <summary>
        /// Filter the action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected Action<T> FilterAction<T>(Action<T> action, Func<T, bool> filter) => x =>
        {
            // If filter returns true, then execute the action
            if (filter(x))
            {
                action(x);
            }
            else
            {
                // Otherwise execute the empty action
                EmptyAction<T>()(x);
            }
        };

        /// <summary>
        /// Wrap the action as async
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected Action<T> WrapAsync<T>(Action<T> action) => x => Task.Run(() => action(x));

        /// <summary>
        /// Empty filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static Func<T, bool> EmptyFilter<T>() => _ => true;
        
        /// <summary>
        /// Empty Action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static Action<T> EmptyAction<T>() => x => { };

        /// <summary>
        /// Creates a new instance of self
        /// </summary>
        /// <returns></returns>
        public static TBuilder New() => new TBuilder();

        /// <summary>
        /// Execute the action and return
        /// </summary>
        /// <param name="return"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected static TS Run<TS>(TS @return, params Action[] action) => Run(() => @return, action);
        
        /// <summary>
        /// Execute the action and return
        /// </summary>
        /// <param name="return"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static TS Run<TS>(Func<TS> @return, params Action[] action)
        {
            action.ForEach(x => x());
            
            return @return();
        }

        /// <summary>
        /// Before build
        /// </summary>
        /// <returns></returns>
        protected abstract void BeforeBuild();

        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public abstract TDestination Build();
    }
}