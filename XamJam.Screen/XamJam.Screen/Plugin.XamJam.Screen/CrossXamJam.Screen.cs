using Plugin.XamJam.Screen.Abstractions;
using System;

namespace Plugin.XamJam.Screen
{
  /// <summary>
  /// Cross platform XamJam.Screen implemenations
  /// </summary>
  public class CrossXamJam.Screen
  {
    static Lazy<IXamJam.Screen> Implementation = new Lazy<IXamJam.Screen>(() => CreateXamJam.Screen(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IXamJam.Screen Current
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

    static IXamJam.Screen CreateXamJam.Screen()
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
