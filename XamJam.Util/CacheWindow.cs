using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Plugin.XamJam.BugHound;

namespace XamJam.Util
{
    public interface DataProvider<T>
    {
        Task<T[]> ProvideAsync(int numItems);
    }

    public class CacheWindow<T>
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(CacheWindow<T>));

        /// <summary>
        /// A local cache that pre-fetches items before the user views them to make certain that network lag is kept to a minimum. 
        /// </summary>
        private readonly LinkedList<T> cache = new LinkedList<T>();

        public int MaxCacheSize { get; }

        private readonly DataProvider<T> dataProvider;

        private readonly BufferBlock<CacheWindowMoved> cursorDeltaBlock = new BufferBlock<CacheWindowMoved>();

        private readonly object lockLast = new object();

        private readonly CancellationToken onShutdown = new CancellationToken();

        private Cursor<T> cursor;

        public CacheWindow(DataProvider<T> dataProvider, int initialCacheSize = 100, int maxCacheSize = 500)
        {
            this.dataProvider = dataProvider;
            MaxCacheSize = maxCacheSize;

            // Populate the cache with the initial data synchronously
            var initialData = dataProvider.ProvideAsync(initialCacheSize).GetAwaiter().GetResult();
            foreach (var initial in initialData)
                cache.AddLast(initial);

            CacheInBackground(initialCacheSize);
        }

        public RetrievedData<T> RetrieveInitialData(int numItemsToRetrieve)
        {
            var tmp = cache.First;
            var initialData = new T[numItemsToRetrieve];
            var currentIndex = 0;
            for (; currentIndex < numItemsToRetrieve - 1; currentIndex++)
            {
                initialData[currentIndex] = tmp.Value;
                if (tmp.Next == null)
                    break;
                tmp = tmp.Next;
            }
            initialData[currentIndex] = tmp.Value;
            cursor = new Cursor<T>(cache.First, 0, tmp, currentIndex);
            Array.Resize(ref initialData, currentIndex + 1);
            //let the background cacher get to work
            cursorDeltaBlock.Post(new CacheWindowMoved(cursor.FirstIndex, cursor.LastIndex, 0));
            return new RetrievedData<T>(initialData);
        }

        private static readonly RetrievedData<T> Empty = new RetrievedData<T>(new T[0]);

        public RetrievedData<T> TryNext(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var i = 0;
            if (!cursor.TryMoveForward(numItemsToRetrieve, item => retrieved[i++] = item))
            {
                return Empty;
            }

            Array.Resize(ref retrieved, i);
            //we've moved the cursor forward by i, let the background cacher get to work
            cursorDeltaBlock.Post(new CacheWindowMoved(cursor.FirstIndex, cursor.LastIndex, i));
            return new RetrievedData<T>(retrieved);
        }

        public RetrievedData<T> TryPrevious(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var i = 0;
            if (!cursor.TryMoveBackward(numItemsToRetrieve, item => retrieved[i++] = item))
            {
                return Empty;
            }

            Array.Resize(ref retrieved, i);
            Array.Reverse(retrieved);
            //we've moved the cursor forward by i, let the background cacher get to work
            cursorDeltaBlock.Post(new CacheWindowMoved(cursor.FirstIndex, cursor.LastIndex, -i));
            return new RetrievedData<T>(retrieved);
        }

        /// <summary>
        /// Maintains the cache window's data by adding data when needed and removing old data when it's out of the window. Except for the ctor, this is
        /// the only method that's allowed to add items to or remove items from the cache.
        /// </summary>
        private void CacheInBackground(int initialCacheSize)
        {
            Task.Run(async () =>
            {
                // First: Populate the full cache
                var numToPopulate = MaxCacheSize - initialCacheSize;
                var fullInitialCache = await dataProvider.ProvideAsync(numToPopulate);
                lock (lockLast)
                {
                    foreach (var c in fullInitialCache)
                    {
                        cache.AddLast(c);
                    }
                    Monitor.Debug($"Populated Initial Cache with {cache.Count} items = {initialCacheSize} + {numToPopulate}");
                }

                // Second: Every time the user pages forward or backward update our cache to add new items and, as needed, remove old items
                while (await cursorDeltaBlock.OutputAvailableAsync(onShutdown))
                {
                    var userMovement = await cursorDeltaBlock.ReceiveAsync(onShutdown);
                    // If the user moved forward, add these items to the end of the cache
                    if (userMovement.IsForward)
                    {
                        var newData = await dataProvider.ProvideAsync(userMovement.Delta);
                        lock (lockLast)
                        {
                            Monitor.Debug($"Adding {newData.Length} items to the back of the cache, current size = {cache.Count}");
                            foreach (var newGuy in newData)
                            {
                                cache.AddLast(newGuy);
                            }
                        }

                        //// If we have too much in the cache, remove from the front
                        //if (cache.Count > MaxCacheSize)
                        //{
                        //    lock (lockFirst)
                        //    {
                        //        var prePurgeSize = cache.Count;
                        //        var numToPurge = prePurgeSize - MaxCacheSize;
                        //        for (var i = 0; i < numToPurge; i++)
                        //        {
                        //            cache.RemoveFirst();
                        //        }
                        //        Monitor.Debug($"Purged {numToPurge} Items from Cache. Cache had {prePurgeSize} items, now has {cache.Count} items. Max cache size = {MaxCacheSize}");
                        //    }
                        //}
                    }
                }
            }, onShutdown);
        }
    }

    public class RetrievedData<T>
    {
        public T[] Retrieved { get; }

        public RetrievedData(T[] retrieved)
        {
            Retrieved = retrieved;
        }
    }

    /// <summary>
    /// Keeps track of where in the cache the user is currently looking, i.e. the cursor or view. This is used to make sure the cache 
    /// is sufficiently populated around the user's view so that when they swipe more data is available in RAM and need not await a network request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cursor<T>
    {
        public LinkedListNode<T> First { get; private set; }

        public LinkedListNode<T> Last { get; private set; }

        public int FirstIndex { get; private set; }

        public int LastIndex { get; private set; }

        public Cursor(LinkedListNode<T> first, int firstIndex, LinkedListNode<T> last, int lastIndex)
        {
            First = first;
            Last = last;
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
        }

        public bool HasNext()
        {
            return Last.Next != null;
        }

        public bool HasPrevious()
        {
            return First.Previous != null;
        }

        public bool TryMoveForward(int desiredNumMoves, Action<T> consumer)
        {
            if (Last.Next == null)
                return false;
            else
            {
                var numActualMoves = 0;
                // First move forward until we reach desiredNumMoves or have no more data. This sets up 'Last' and 'LastIndex', and numActualMoves.
                for (; numActualMoves < desiredNumMoves; numActualMoves++)
                {
                    // If we can't move forward anymore break
                    if (Last.Next == null)
                        break;
                    Last = Last.Next;
                    LastIndex++;
                    // consume the new data
                    consumer(Last.Value);
                }
                // Now use the Last information to backtrack and figure out First and FirstIndex
                numActualMoves--;
                FirstIndex = LastIndex - numActualMoves;
                First = Last;
                for (var i = 0; i < numActualMoves; i++)
                    First = First.Previous;
                return true;
            }
        }

        public bool TryMoveBackward(int desiredNumMoves, Action<T> consumer)
        {
            if (First.Previous == null)
                return false;
            else
            {
                var numActualMoves = 0;
                // First move back until we can't we reach desiredNumMoves or have no more data. This sets up 'First', 'FirstIndex', and 'numActualMoves'. 
                for (; numActualMoves < desiredNumMoves; numActualMoves++)
                {
                    // If we can't move back anymore break
                    if (First.Previous == null)
                        break;
                    First = First.Previous;
                    FirstIndex--;
                    // consume the previously seen data
                    consumer(First.Value);
                }
                // Now that we know 'First', 'FirstIndex', and numActualMoves figure out 'Last' and 'LastIndex' 
                numActualMoves--;
                LastIndex = FirstIndex + numActualMoves;
                Last = First;
                for (var i = 0; i < numActualMoves; i++)
                    Last = Last.Next;
                return true;
            }
        }

        public override string ToString()
        {
            return $"{FirstIndex} - {LastIndex}. {First.Value} - {Last.Value}";
        }
    }

    /// <summary>
    /// This event is raised everytime the user moved the cursor/view so that the background cacher can both populate new cache values and trim old ones.
    /// </summary>
    internal struct CacheWindowMoved
    {
        /// <summary>
        /// The new First Index in the Cache's Cursor/View
        /// </summary>
        public int FirstIndex { get; }

        public int LastIndex { get; }

        public int Delta { get; }
        public bool IsForward => Delta > 0;

        public CacheWindowMoved(int firstIndex, int lastIndex, int delta)
        {
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
            Delta = delta;
        }
    }
}
