#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace XamJam.Util
{
    /// <summary>
    ///     Holds two windows of data: next & previous. Uses a background thread to pre-cache the next window.
    ///     Has a very limited peeking functionality, one can only peek starting at index 0 and moving forward single index by
    ///     single index.
    /// </summary>
    public class DataWindow<TData>
    {
        private static readonly Task<bool> TrueTask = Task.FromResult(true);
        private static readonly Task<bool> FalseTask = Task.FromResult(false);
        private readonly FixedSizedQueue<TData> previousQ, nextQ;
        private readonly IDataProvider<TData> provider;
        private readonly int windowSize;
        private bool shutdown;

        public DataWindow(IDataProvider<TData> provider, int windowSize)
        {
            this.provider = provider;
            this.windowSize = windowSize;
            previousQ = new FixedSizedQueue<TData>(windowSize);
            nextQ = new FixedSizedQueue<TData>(windowSize);
            Task.Run(() => CacheInBackground());
        }

        public TData Current => nextQ.First;

        public bool CanPeek(int offset)
        {
            return nextQ.CanPeek(offset);
        }

        public TData Peek(int offset)
        {
            return nextQ.Peek(offset);
        }

        public Task<bool> HasNextAsync()
        {
            return nextQ.HasSomething ? TrueTask : provider.CanProvideAsync();
        }

        public Task<bool> HasPreviousAsync(bool goBackwardsForever = true)
        {
            if (previousQ.HasSomething)
                return TrueTask;
            if (goBackwardsForever)
                return provider.CanProvideAsync();
            return FalseTask;
        }

        public async Task<TData> NextAsync()
        {
            if (nextQ.IsEmpty)
            {
                var n = await provider.ProvideAsync();
                nextQ.Enqueue(n);
            }
            var next = nextQ.Dequeue();
            previousQ.Push(next);
            return next;
        }

        public async Task<TData> PreviousAsync(bool goBackwardsForever = true)
        {
            TData prev;
            if (previousQ.IsEmpty && goBackwardsForever)
                prev = await provider.ProvideAsync();
            else
                prev = previousQ.Dequeue();
            nextQ.Push(prev);
            return prev;
        }

        public void Shutdown()
        {
            shutdown = true;
            //System.Threading.Monitor.PulseAll(backgroundCacheWait);
        }

        private void CacheInBackground()
        {
            while (!shutdown)
            {
                if (nextQ.Count < windowSize)
                {
                    //only pre-cache the 'nextQ', previous comes from next...
                    while (nextQ.Count < windowSize)
                    {
                        provider.ProvideAsync().ContinueWith(n => nextQ.Enqueue(n.Result));
                        //var next = await provider.ProvideAsync();
                        ///nextQ.Enqueue(next);
                    }
                }
                // every 5,000 seconds see if we can download more info. Shutdown happens immediately, breaking this timeout via Monitor.PulseAll in the shutdown method.
                //System.Threading.Monitor.Wait(backgroundCacheWait, 5000);
            }
        }

        /// <summary>
        ///     A very limited but efficient fixed-sized queue. It can only peek by starting at 0 and moving forward.
        /// </summary>
        private class FixedSizedQueue<TDataType>
        {
            private readonly int limit;
            private readonly LinkedList<TDataType> q = new LinkedList<TDataType>();
            private LinkedListNode<TDataType> currentPeek;

            private int currentPeekIndex = -2;

            public FixedSizedQueue(int limit)
            {
                this.limit = limit;
            }

            public TDataType First => q.First.Value;

            public bool IsEmpty => q.Count == 0;

            public bool HasSomething => q.Count != 0;

            public int Count => q.Count;

            public void Enqueue(TDataType enqueueMe)
            {
                if (q.Count == limit)
                {
                    q.RemoveLast();
                }
                q.AddLast(enqueueMe);
            }

            public void Push(TDataType enqueueMe)
            {
                if (q.Count == limit)
                {
                    q.RemoveLast();
                }
                q.AddFirst(enqueueMe);
            }

            public TDataType Dequeue()
            {
                var first = q.First.Value;
                q.RemoveFirst();
                return first;
            }

            public bool CanPeek(int index)
            {
                return index < q.Count;
            }

            public TDataType Peek(int index)
            {
                // First peek, setup the cache
                if (index == 0)
                {
                    currentPeekIndex = index;
                    currentPeek = q.First;
                    return currentPeek.Value;
                }
                // Typical case, we're looking at the next peek
                if (currentPeekIndex + 1 == index)
                {
                    currentPeekIndex++;
                    currentPeek = currentPeek.Next;
                    return currentPeek.Value;
                }
                throw new InvalidOperationException("You forgot to reset the peek cache");
            }
        }
    }

    public interface IDataProvider<TData>
    {
        Task<bool> CanProvideAsync();

        Task<TData> ProvideAsync();
    }
}