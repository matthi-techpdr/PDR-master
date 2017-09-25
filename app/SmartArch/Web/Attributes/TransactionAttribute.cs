using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using SmartArch.Core.Helpers.EntityLocalization;
using SmartArch.Data;

namespace SmartArch.Web.Attributes
{
    /// <summary>
    /// Sharp architecture attribute
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TransactionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The helper's engine.
        /// </summary>
        private static ITransactionManager engine;

        /// <summary>
        /// Gets a value indicating whether this instance has default engine.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has default engine; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDefaultEngine { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="EntityLocalizationHelper"/> class.
        /// </summary>
        static TransactionAttribute()
        {
            SetDefaultEngine();
        }

        /// <summary>
        /// Gets or sets the engine.
        /// </summary>
        /// <value>The helper's engine.</value>
        public static ITransactionManager Engine
        {
            get
            {
                // return engine ?? (engine = ServiceLocator.Current.GetInstance<ITransactionManager>());
                return ServiceLocator.Current.GetInstance<ITransactionManager>();
            }

            set
            {
                engine = value;
                IsDefaultEngine = false;
            }
        }

        /// <summary>
        /// Sets the default engine.
        /// </summary>
        public static void SetDefaultEngine()
        {
            engine = null;
            IsDefaultEngine = true;
        }
        
        #region Methods
        /// <summary>
        /// Called by the MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Engine.IsActive && ((filterContext.Exception != null) && filterContext.ExceptionHandled))
            {
                Engine.Rollback();
            }
        }

        /// <summary>
        /// Called by the MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Engine.Begin();
        }

        /// <summary>
        /// Called by the MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            try
            {
                if (Engine.IsActive)
                {
                    if ((filterContext.Exception != null) && !filterContext.ExceptionHandled)
                    {
                        Engine.Rollback();
                    }
                    else
                    {
                        Engine.Commit();
                    }
                }
            }
            finally
            {
                Engine.Dispose();
            }
        }
        #endregion
    }
}