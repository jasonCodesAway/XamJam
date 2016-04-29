#region

using System;
using System.Collections.Generic;

#endregion

namespace XamJam.Util
{
    public static class ListExtensionMethods
    {
        private static readonly Random rng = new Random();

        public static T Random<T>(this List<T> list)
        {
            return list[rng.Next(list.Count)];
        }

        public static T NewRandom<T>(this List<T> list, T oldT)
        {
            if (list.Count == 1 && list.Contains(oldT))
            {
                throw new ArgumentException("Cannot generate a new random value, there is only 1 value in the list");
            }
            var newT = list[rng.Next(list.Count)];
            while (EqualityComparer<T>.Default.Equals(newT, oldT))
            {
                newT = list[rng.Next(list.Count)];
            }
            return newT;
        }
    }
}