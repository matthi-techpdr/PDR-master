using System;

namespace SmartArch.Core.Helpers
{
    /// <summary>
    /// Wrapper for <c>DateTime</c> methods
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        /// The engine of getting current date time.
        /// </summary>
        public static Func<DateTime> Engine { private get; set; }

        /// <summary>
        /// Initializes static members of the <see cref="SystemTime"/> class. 
        /// </summary>
        static SystemTime()
        {
            SetDefaultEngine();
        }

        /// <summary>
        /// Sets the default engine.
        /// </summary>
        public static void SetDefaultEngine()
        {
            Engine = () => DateTime.Now;
        }

        /// <summary>
        /// Return date time for this moment
        /// </summary>
        public static Func<DateTime> Now
        {
            get
            {
                return Engine;
            }
        }
    }
}