using System.Threading;
using Plugin.XamJam.BugHound.Abstractions;

namespace Plugin.XamJam.BugHound
{
    /// <summary>
    /// <see cref="IBugHoundHelper"/>
    /// </summary>
    public class BugHoundHelperImplementation : IBugHoundHelper
    {
        /// <summary>
        /// <see cref="IBugHoundHelper.ThreadName"/>
        /// </summary>
        public string ThreadName => Thread.CurrentThread.Name;
    }
}