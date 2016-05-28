using Plugin.XamJam.Pic.Abstractions;
using System;

namespace Plugin.XamJam.Pic
{
  /// <summary>
  /// Cross platform XamJam.Pic implemenations
  /// </summary>
  public class PicManager
  {
      private static readonly Lazy<IPicManager> Implementation = new Lazy<IPicManager>(CreatePic, System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IPicManager Current
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

    private static IPicManager CreatePic()
    {
#if PORTABLE
        return null;
#else
        return new PicManagerImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
