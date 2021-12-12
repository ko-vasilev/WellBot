using System;
using System.Collections.Generic;
using System.Linq;

namespace WellBot.UseCases.Common.Extensions
{
    /// <summary>
    /// Contains extension methods for accessing collections in a random way.
    /// </summary>
    public static class EnumerableRandom
    {
        private static Random random = new Random();

        /// <summary>
        /// Pick a random element from a collection.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <returns>Random element.</returns>
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            int elementIndex = random.Next(source.Count());
            return source.ElementAt(elementIndex);
        }

        /// <summary>
        /// Pick a random element from a list.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <returns>Random element.</returns>
        public static T PickRandom<T>(this IList<T> source)
        {
            int elementIndex = random.Next(source.Count);
            return source[elementIndex];
        }

        /// <summary>
        /// Get a random value from 0 to <paramref name="max"/>.
        /// </summary>
        /// <param name="max">Exclusive max value.</param>
        /// <returns>Random value.</returns>
        public static int GetRandom(int max) => random.Next(max);
    }
}
