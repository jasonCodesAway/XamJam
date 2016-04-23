using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Plugin.XamJam.BugHound.Abstractions;

namespace Plugin.XamJam.BugHound
{
    /// <summary>
    /// This is the BugHound, over.
    /// </summary>
    public class BugHound : IBugHound
    {
        private static readonly ConcurrentDictionary<Type, IBugHound> ByTypeMap = new ConcurrentDictionary<Type, IBugHound>();

        private static readonly Stopwatch Watch = new Stopwatch();

        static BugHound()
        {
            Watch.Start();
        }

        /// <summary>
        /// Create a BugHound for a specific Type
        /// </summary>
        public static IBugHound ByType(Type type, Level level = null)
        {
            // By default this hound is @ Debug during Debug and Info during Release
            if (level == null)
            {
#if DEBUG
                level = Level.Debug;
#else
                level = Level.Info;
#endif
            }
            return ByTypeMap.GetOrAdd(type, t => new BugHound(BugHoundHelper.Helper, level, t));
        }

        /// <summary>
        /// This BugHound's current log level
        /// </summary>
        public Level Level { get; set; }

        private readonly Type type;
        private readonly IBugHoundHelper helper;

        internal BugHound(IBugHoundHelper helper, Level level, Type type)
        {
            this.helper = helper;
            this.type = type;
            Level = level;
        }

        /// <summary>
        /// <see cref="IBugHound.Trace"/>
        /// </summary>
        public void Trace(string message)
        {
            Log(Level.Trace, message);
        }

        /// <summary>
        /// <see cref="IBugHound.Debug"/>
        /// </summary>
        public void Debug(string message)
        {
            Log(Level.Debug, message);
        }

        /// <summary>
        /// <see cref="IBugHound.Info"/>
        /// </summary>
        public void Info(string message)
        {
            Log(Level.Info, message);
        }

        /// <summary>
        /// <see cref="IBugHound.Warn"/>
        /// </summary>
        public void Warn(string message, Exception ex = null)
        {
            Log(Level.Warn, message, ex);
        }

        /// <summary>
        /// <see cref="IBugHound.Error"/>
        /// </summary>
        public void Error(string message, Exception ex = null)
        {
            Log(Level.Error, message, ex);
        }

        /// <summary>
        /// <see cref="IBugHound.Log"/>
        /// </summary>
        public void Log(Level level, string message, Exception ex = null)
        {
            if (level >= Level)
            {
                var logMsg = $"{Watch.ElapsedMilliseconds,-10} | {level.Label,-5} | ";
                // Looks like Xamarin or Xamarin Studio is inserting the time in their format and I can't do anything about it. So, don't do it here, at least for now.
                // Msg += string.Format ("{0,-2}:{1,-2}:{2,-2}.{3,-3} | ", logTime.Hour, logTime.Minute, logTime.Second, logTime.Millisecond);
                var threadName = helper?.ThreadName;
                if (threadName != null)
                {
                    logMsg += $"{threadName} | ";
                }
                if (type != null)
                {
                    logMsg += $"{type.Name} : ";
                }
                logMsg += message;
                if (ex != null)
                {
                    logMsg += Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                }
                System.Diagnostics.Debug.WriteLine(logMsg);
            }
        }

        /// <summary>
        /// <see cref="IBugHound.Throw"/>
        /// </summary>
        public void Throw(string exceptionMessage = null, Exception ex = null, Level level = null)
        {
            Log(level ?? Level.Error, exceptionMessage, ex);
            throw new Exception(exceptionMessage, ex);
        }
    }
}
