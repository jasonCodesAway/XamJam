using Plugin.XamJam.Screen.Abstractions;
using System;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Cross platform XamJam.CrossScreen implemenations
    /// </summary>
    public class CrossScreen
    {
        private static readonly Lazy<Abstractions.Screen> Implementation = new Lazy<Abstractions.Screen>(CreateScreen, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static Abstractions.Screen Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        private static Abstractions.Screen CreateScreen()
        {
#if PORTABLE
            return null;
#else
        return new XamJam.ScreenImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
