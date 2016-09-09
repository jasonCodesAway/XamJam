namespace Plugin.XamJam.BugHound.Abstractions
{
    /// <summary>
    /// Resolves all platform-dependent functionality required by the <see cref="Plugin.XamJam.BugHound"/>
    /// </summary>
    public interface IBugHoundHelper
    {
        /// <summary>
        /// Returns a unique identifier for the current thread, or null if the platform doesn't allow access to a unique thread-based identifier.
        /// </summary>
        string ThreadName { get; }
    }
}
