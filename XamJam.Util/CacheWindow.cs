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

        private readonly DataProvider<T> dataProvider;

        private readonly BufferBlock<int> cursorDeltaBlock = new BufferBlock<int>();

        private readonly object lockLast = new object(), lockFirst = new object();

        private readonly CancellationToken onShutdown = new CancellationToken();

        public CacheWindow(DataProvider<T> dataProvider, int cacheSize = 100)
        {
            this.dataProvider = dataProvider;
            Size = cacheSize;
            HalfSize = Size / 2;
            // Now populate our intial HalfSize cache
            var initialData = dataProvider.ProvideAsync(HalfSize).GetAwaiter().GetResult();
            cursor = cache.AddFirst(initialData[0]);
            for (var i = 1; i < initialData.Length; i++)
                cache.AddLast(initialData[i]);
            // Setup the cursor to point 
            //cursor = cache.AddLast(initialData[initialData.Length-1]);
            CacheInBackground();
        }

        public RetrievedData<T> TryNext(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var i = 0;
            do
            {
                retrieved[i] = cursor.Value;
                if (cursor.Next != null)
                {
                    cursor = cursor.Next;
                    i++;
                }
                else
                    break;
            } while (i < numItemsToRetrieve);
            Array.Resize(ref retrieved, i);
            //we've moved the cursor forward by i-1, let the background cacher get to work
            cursorDeltaBlock.Post(i - 1);
            return new RetrievedData<T>(retrieved);
        }

        public RetrievedData<T> TryPrevious(int numItemsToRetrieve)
        {
            throw new Exception("TODO");
            //if (cursor.Previous == null || cursor.Previous.Value == null)
            //{
            //    return new RetrievedData<T>(false, default(T));
            //}
            //else
            //{
            //    cursor = cursor.Previous;
            //    cursorIndex--;
            //    return new RetrievedData<T>(true, cursor.Value);
            //}
        }

        private void CacheInBackground()
        {
            Task.Run(async () =>
            {
                while (await cursorDeltaBlock.OutputAvailableAsync(onShutdown))
                {
                    var cursorDelta = cursorDeltaBlock.Receive(onShutdown);
                    // User moved forward by 'cursorDelta'
                    if (cursorDelta > 0)
                    {
                        var newData = await dataProvider.ProvideAsync(cursorDelta);
                        lock (lockLast)
                        {
                            Monitor.Debug($"Adding {newData.Length} items to the cache");
                            foreach (var newGuy in newData)
                            {
                                cache.AddLast(newGuy);
                            }
                        }

                        // Delete extra
                        lock (lockFirst)
                        {
                            for (var i = 0; i < cursorDelta; i++)
                                cache.RemoveFirst();
                        }
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
