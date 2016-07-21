using System;
using System.Diagnostics;
using Plugin.XamJam.BugHound.Abstractions;

namespace Plugin.XamJam.BugHound
{
    /// <summary>
    /// Cross platform BugHoundHelper
    /// </summary>
    public class BugHoundHelper
    {
        private static readonly Lazy<IBugHoundHelper> Implementation = new Lazy<IBugHoundHelper>(CreateBugHound, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        ///     Forces the creation of the platform-dependent <see cref="IBugHoundHelper"/> implementation.
        /// </summary>
        public static IBugHoundHelper Helper
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    Debug.WriteLine("WARN: Failed to find 'Xam.Plugins.XamJam.BugHound' in your startup platform project's (e.g. Droid, iOS, WP, UWP, etc.) references. Falling back to platform-agnostic logging.");
                }
                return ret;
            }

        }

        private static IBugHoundHelper CreateBugHound()
        {
#if PORTABLE
            return null;
#else
        return new BugHoundHelperImplementation();
#endif
        }
    }
}
