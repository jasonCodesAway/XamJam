using Plugin.XamJam.BugHound.Abstractions;
using System;
using Java.Lang;


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
        public string ThreadName => Thread.CurrentThread().Name;
    }
}