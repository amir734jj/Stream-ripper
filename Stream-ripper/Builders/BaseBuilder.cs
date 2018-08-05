using System;
using System.Threading.Tasks;
using StreamRipper.Extensions;

namespace StreamRipper.Builders
{
    /// <summary>
    /// Base builder, needed so that fields of mappers are re-created during each mapping process
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <typeparam name="TArg"></typeparam>
    public abstract class BaseBuilder<TBuilder, TArg>
    {
        /// <summary>
        /// Creates a new instance of self
        /// </summary>
        /// <returns></returns>
        public static TBuilder New(TArg arg) => (TBuilder) Activator.CreateInstance(typeof(TBuilder), arg);

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
    }
}