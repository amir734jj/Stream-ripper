using System;
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