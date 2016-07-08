#region

using System;

#endregion

namespace XamJam.Util
{
    /// <summary>
    ///     Used to coalesce many updates into one update. So, if an action like redrawing a window needs to be done, but only
    ///     done every so often, this class can make sure
    ///     that action is only
    /// </summary>
    public class Coalesce
    {
        private readonly TimeSpan period;
        private readonly object syncLock = new object();
        private readonly Action<TimeSpan, Func<bool>> timer;
        private readonly Func<bool> update;

        private bool hasInitialized, isDirty = true;

        /// <summary>
        /// </summary>
        /// <param name="coalescePeriod"></param>
        /// <param name="update">
        ///     the code that will actually do the update, will keep getting called periodically until this
        ///     returns false
        /// </param>
        /// <param name="timer">Takes in a time period and executes</param>
        public Coalesce(TimeSpan coalescePeriod, Func<bool> update, Action<TimeSpan, Func<bool>> timer)
        {
            period = coalescePeriod;
            this.update = update;
            this.timer = timer;
        }

        public void NeedsUpdate()
        {
            isDirty = true;
            lock (syncLock)
            {
                if (!hasInitialized)
                {
                    hasInitialized = true;
                    timer(period, () =>
                    {
                        var keepGoing = true;
                        lock (syncLock)
                        {
                            if (isDirty)
                            {
                                keepGoing = update();
                                isDirty = false;
                            }
                        }
                        return keepGoing;
                    });
                }
            }
        }
    }
}