using System;
using System.Collections.Concurrent;
using Plugin.XamJam.BugHound;

namespace XamJam.Util
{
    public class ConcurrentLruCache<TK, TV>
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof (ConcurrentLruCache<TK, TV>));
        private readonly ConcurrentDictionary<TK, TV> map = new ConcurrentDictionary<TK, TV>();
        private readonly ConcurrentQueue<TK> q = new ConcurrentQueue<TK>();

        public ConcurrentLruCache(int capacity)
        {
            Capacity = capacity;
            if (Capacity <= 0)
                throw new ArgumentException("Cannot have a empty LruCache");
        }

        public int Capacity { get; }

        public TV this[TK key] => map[key];

        public void Add(TK newKey, TV newValue)
        {
            if (map.Count == Capacity)
            {
                TK removedKey;
                if (q.TryDequeue(out removedKey))
                {
                    TV removedValue;
                    map.TryRemove(removedKey, out removedValue);
                    if (removedValue == null)
                    {
                        Monitor.Throw(
                            $"Failed to remove key: {removedKey}, Queue size: {q.Count}, Map size: {map.Count}");
                    }
                }
            }
            q.Enqueue(newKey);
            map.AddOrUpdate(newKey, key => newValue, (key, oldValue) => newValue);
        }

        public TV Get(TK key, Func<TV> provider)
        {
            if (!map.ContainsKey(key))
            {
                Add(key, provider());
            }
            return map[key];
        }

        public bool ContainsKey(TK key)
        {
            return map.ContainsKey(key);
        }
    }
}