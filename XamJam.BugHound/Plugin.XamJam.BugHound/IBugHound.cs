using System;

namespace Plugin.XamJam.BugHound
{
  /// <summary>
  /// Interface for XamJam.BugHound
  /// </summary>
  public interface IBugHound
  {
        /// <summary>
        /// <see cref="Level.Trace"/>
        /// </summary>
        /// <param name="message">the log message</param>
        void Trace(string message);

        /// <summary>
        /// <see cref="Level.Debug"/>
        /// </summary>
        /// <param name="message">the log message</param>
        void Debug(string message);

        /// <summary>
        /// <see cref="Level.Info"/>
        /// </summary>
        /// <param name="message">the log message</param>
        void Info(string message);

        /// <summary>
        /// <see cref="Level.Warn"/>
        /// </summary>
        /// <param name="message">the log message</param>
        /// <param name="ex">the optional applicable exception</param>
        void Warn(string message, Exception ex = null);

        /// <summary>
        /// <see cref="Level.Error"/>
        /// </summary>
        /// <param name="message">the log message</param>
        /// <param name="ex">the optional applicable exception</param>
        void Error(string message, Exception ex = null);

      /// <summary>
      /// Logs a message and, optionally, an exception, at the specified level.
      /// </summary>
      /// <param name="level">the log level for this log message</param>
      /// <param name="message">the log message</param>
      /// <param name="ex">the optional applicable exception</param>
      void Log(Level level, string message, Exception ex = null);

        /// <summary>
        /// The Hound records the exception at <see cref="Level.Error"/> level and rethrows it.
        /// </summary>
        /// <param name="exceptionMessage">a custom message helping diagnose the exception</param>
        /// <param name="ex">the inner (aka caused-by) exception, if any</param>
        /// <param name="level">a custom level other than the default <see cref="Level.Error"/></param>
        void Throw(string exceptionMessage = null, Exception ex = null, Level level = null);
    }
}
