namespace Plugin.XamJam.BugHound
{
    /// <summary>
    ///     Log levels
    /// </summary>
    public sealed class Level
    {
        /// <summary>
        /// The lowest standard log level, very fine-grained logging here. Typically floods logs.
        /// </summary>
        public static readonly Level Trace = new Level("Trace", 0);

        /// <summary>
        /// A low standard log level, fine-grained logging here. Typically results in large logs.
        /// </summary>
        public static readonly Level Debug = new Level("Debug", 1);

        /// <summary>
        /// A medium standard log level, normal-grained logging here. Typically results in acceptable/normal log sizes.
        /// </summary>
        public static readonly Level Info = new Level("Info", 2);

        /// <summary>
        /// A high standard log level, coarse-grained logging here. Typically results in small logs.
        /// </summary>
        public static readonly Level Warn = new Level("Warn", 3);

        /// <summary>
        /// A very high standard log level, very coarse-grained logging here. Typically results in very small logs.
        /// </summary>
        public static readonly Level Error = new Level("Error", 4);

        private readonly byte levelCode;

        private Level(string label, byte levelCode)
        {
            Label = label;
            this.levelCode = levelCode;
        }

        /// <summary>
        /// A user-friendly label for this log level
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Less-than log level operator
        /// </summary>
        /// <param name="a">log level a</param>
        /// <param name="b">log level b</param>
        /// <returns>true if a is less than b, e.g. true if a = Trace and b = Error</returns>
        public static bool operator <(Level a, Level b)
        {
            return a.levelCode < b.levelCode;
        }

        /// <summary>
        /// Greater-than log level operator
        /// </summary>
        /// <param name="a">log level a</param>
        /// <param name="b">log level b</param>
        /// <returns>true if a is greater than b, e.g. true if a = Error and b = Trace</returns>
        public static bool operator >(Level a, Level b)
        {
            return a.levelCode > b.levelCode;
        }

        /// <summary>
        /// Less-than or equal log level operator
        /// </summary>
        /// <param name="a">log level a</param>
        /// <param name="b">log level b</param>
        /// <returns>true if a is less than or equal to b, e.g. true if a = Trace and b = Trace</returns>
        public static bool operator <=(Level a, Level b)
        {
            return a.levelCode <= b.levelCode;
        }

        /// <summary>
        /// Greater-than or equal to log level operator
        /// </summary>
        /// <param name="a">log level a</param>
        /// <param name="b">log level b</param>
        /// <returns>true if a is greater than or equal to b, e.g. true if a = Error and b = Error</returns>
        public static bool operator >=(Level a, Level b)
        {
            return a.levelCode >= b.levelCode;
        }
    }
}
