#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace XamJam.Util
{
    /// <summary>
    ///     A dirt-simple non-thread-safe max capacity lru cache
    /// </summary>
    public class LruCache<TK, TV>
    {
        private readonly Dictionary<TK, TV> map;
        private readonly LinkedList<TK> q = new LinkedList<TK>();

        public LruCache(int capacity)
        {
            Capacity = capacity;
            if (Capacity <= 0)
                throw new ArgumentException("Cannot have a empty LruCache");
            map = new Dictionary<TK, TV>(capacity);
        }

        public int Capacity { get; }

        public TV this[TK key] => map[key];

        public void Add(TK key, TV value)
        {
            if (map.Count == Capacity)
            {
                map.Remove(q.First.Value);
                q.RemoveFirst();
            }
            q.AddLast(key);
            map[key] = value;
        }

        public TV Get(TK key, Func<TV> provider)
        {
            if (!map.ContainsKey(key))
            {
                Add(key, provider());
            }
            return map[key];
        }

        public async Task<TV> GetAsync(TK key, Func<Task<TV>> provider)
        {
            if (!map.ContainsKey(key))
            {
                var value = await Task.Run(async () => await provider());
                Add(key, value);
            }
            return map[key];
        }


        public bool ContainsKey(TK key)
        {
            return map.ContainsKey(key);
        }
    }
}