using System;
using System.Collections.Generic;
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

        public int Size { get; }

        public int HalfSize { get; }

        /// <summary>
        /// The local cache that's loaded into RAM
        /// </summary>
        private readonly LinkedList<T> cache = new LinkedList<T>();

        /// <summary>
        /// The visible/current/active node in the cache
        /// </summary>
        private LinkedListNode<T> cursor;

        private int cursorIndex = 0;

        private readonly DataProvider<T> dataProvider;

        private readonly BufferBlock<Tuple<int, int>> cursorDeltaBlock = new BufferBlock<Tuple<int, int>>();

        private readonly object lockLast = new object(), lockFirst = new object();

        private readonly CancellationToken onShutdown = new CancellationToken();

        public CacheWindow(DataProvider<T> dataProvider, int initialCacheSize = 100, int cacheSize = 500)
        {
            this.dataProvider = dataProvider;
            Size = cacheSize;
            HalfSize = Size / 2;
            CacheInBackground(initialCacheSize);
        }

        public RetrievedData<T> RetrieveInitialData(int numItemsToRetrieve)
        {
            var initialData = new T[numItemsToRetrieve];
            cursor = cache.First;
            for (cursorIndex = 0; cursorIndex < initialData.Length - 1;)
            {
                initialData[cursorIndex] = cursor.Value;
                if (cursor.Next == null)
                    break;
                cursor = cursor.Next;
                cursorIndex++;
            }
            initialData[cursorIndex] = cursor.Value;
            // Setup the cursor to point 
            //cursor = cache.Last;
            Array.Resize(ref initialData, cursorIndex + 1);
            //let the background cacher get to work
            cursorDeltaBlock.Post(new Tuple<int, int>(cursorIndex, cursorIndex));
            return new RetrievedData<T>(initialData);
        }

        private static readonly RetrievedData<T> empty = new RetrievedData<T>(new T[0]);

        public RetrievedData<T> TryNext(int numItemsToRetrieve)
        {
            if (cursor.Next == null)
                return empty;
            cursor = cursor.Next;
            cursorIndex++;

            var retrieved = new T[numItemsToRetrieve];
            var retrievedIndex = 0;
            for (; retrievedIndex < numItemsToRetrieve; retrievedIndex++)
            {
                retrieved[retrievedIndex] = cursor.Value;
                //If no data exists, break out
                if (cursor.Next == null || retrievedIndex == numItemsToRetrieve - 1)
                    break;
                cursor = cursor.Next;
                cursorIndex++;
            }

            Array.Resize(ref retrieved, retrievedIndex + 1);
            //we've moved the cursor forward by i-1, let the background cacher get to work
            cursorDeltaBlock.Post(new Tuple<int, int>(retrievedIndex, cursorIndex));
            return new RetrievedData<T>(retrieved);
        }

        public RetrievedData<T> TryPrevious(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var retrievedIndex = 0;
            while (retrievedIndex < numItemsToRetrieve)
            {
                retrieved[retrievedIndex] = cursor.Value;
                if (cursor.Previous != null)
                {
                    cursor = cursor.Previous;
                    retrievedIndex++;
                    cursorIndex--;
                }
                else
                    break;
            }
            Array.Resize(ref retrieved, retrievedIndex + 1);
            //we've moved the cursor backward by i-1, let the background cacher get to work
            cursorDeltaBlock.Post(new Tuple<int, int>(-retrievedIndex, cursorIndex));
            return new RetrievedData<T>(retrieved);
        }

        /// <summary>
        /// Maintains the cache window's data by adding data when needed and removing old data when it's out of the window
        /// </summary>
        private void CacheInBackground(int initialCacheSize)
        {
            var initialData = dataProvider.ProvideAsync(initialCacheSize).GetAwaiter().GetResult();
            foreach (var initial in initialData)
                cache.AddLast(initial);

            Task.Run(async () =>
            {
                // int numItemsInFront = initialDataLength, numItemsInBack = 0;
                while (await cursorDeltaBlock.OutputAvailableAsync(onShutdown))
                {
                    var change = cursorDeltaBlock.Receive(onShutdown);
                    var cursorDeltaUpdate = change.Item1;
                    var cursorIndexUpdate = change.Item2;
                    // User moved forward by 'cursorDelta'
                    if (cursorDeltaUpdate > 0)
                    {
                        var newData = await dataProvider.ProvideAsync(cursorDeltaUpdate);
                        lock (lockLast)
                        {
                            Monitor.Debug($"Adding {newData.Length} items to the cache");
                            foreach (var newGuy in newData)
                            {
                                cache.AddLast(newGuy);
                                //numItemsInFront++;
                            }
                        }

                        //// Delete extra
                        //lock (lockFirst)
                        //{
                        //    var deleteFirst = cursorIndexUpdate - HalfSize + 1;
                        //    if (deleteFirst > 0)
                        //        Monitor.Debug($"Deleting {deleteFirst} entries from the front of the cache");
                        //    for (var i = 0; i < deleteFirst; i++)
                        //        cache.RemoveFirst();
                        //}
                    }
                    else
                    {
                        //TODO: How to move backward?
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
}
