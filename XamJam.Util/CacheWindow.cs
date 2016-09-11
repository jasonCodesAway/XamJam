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

        private readonly DataProvider<T> dataProvider;

        private readonly BufferBlock<Tuple<int, int>> cursorDeltaBlock = new BufferBlock<Tuple<int, int>>();

        private readonly object lockLast = new object(), lockFirst = new object();

        private readonly CancellationToken onShutdown = new CancellationToken();

        private Cursor<T> cursor;

        public CacheWindow(DataProvider<T> dataProvider, int initialCacheSize = 100, int cacheSize = 500)
        {
            this.dataProvider = dataProvider;
            Size = cacheSize;
            HalfSize = Size / 2;
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
            cursorDeltaBlock.Post(new Tuple<int, int>(currentIndex, currentIndex));
            return new RetrievedData<T>(initialData);
        }

        private static readonly RetrievedData<T> empty = new RetrievedData<T>(new T[0]);

        public RetrievedData<T> TryNext(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var i = 0;
            if (!cursor.TryMoveForward(numItemsToRetrieve, item => retrieved[i++] = item))
            {
                return empty;
            }

            Array.Resize(ref retrieved, i);
            //we've moved the cursor forward by i, let the background cacher get to work
            cursorDeltaBlock.Post(new Tuple<int, int>(i, cursor.LastIndex));
            return new RetrievedData<T>(retrieved);
        }

        public RetrievedData<T> TryPrevious(int numItemsToRetrieve)
        {
            var retrieved = new T[numItemsToRetrieve];
            var i = 0;
            if (!cursor.TryMoveBackward(numItemsToRetrieve, item => retrieved[i++] = item))
            {
                return empty;
            }

            Array.Resize(ref retrieved, i);
            Array.Reverse(retrieved);
            //we've moved the cursor forward by i, let the background cacher get to work
            cursorDeltaBlock.Post(new Tuple<int, int>(-i, cursor.FirstIndex));
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

    public class Cursor<T>
    {
        public LinkedListNode<T> First { get; private set; }

        public LinkedListNode<T> Last { get; private set; }

        public int FirstIndex { get; private set; }

        public int LastIndex { get; private set; }

        public Cursor(LinkedListNode<T> first, int firstIndex, LinkedListNode<T> last, int lastIndex)
        {
            this.First = first;
            this.Last = last;
            this.FirstIndex = firstIndex;
            this.LastIndex = lastIndex;
        }

        public bool HasNext()
        {
            return Last.Next != null;
        }

        public bool HasPrevious()
        {
            return First.Previous != null;
        }

        public bool TryMoveForward(int numMoves, Action<T> consumer)
        {
            if (Last.Next == null)
                return false;
            else
            {
                for (var i = 0; i < numMoves; i++)
                {
                    if (Last.Next == null)
                        break;
                    First = First.Next;
                    FirstIndex++;
                    Last = Last.Next;
                    LastIndex++;
                    // consume the new data
                    consumer(Last.Value);
                }
                return true;
            }
        }

        public bool TryMoveBackward(int numMoves, Action<T> consumer)
        {
            if (First.Previous == null)
                return false;
            else
            {
                for (var i = 0; i < numMoves; i++)
                {
                    if (First.Previous == null)
                        break;
                    First = First.Previous;
                    FirstIndex--;
                    Last = Last.Previous;
                    LastIndex--;
                    // consume the previously seen data
                    consumer(First.Value);
                }
                return true;
            }
        }

        public override string ToString()
        {
            return $"{FirstIndex} - {LastIndex}. {First.Value} - {Last.Value}";
        }
    }

}
