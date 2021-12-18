using System;
using System.Collections.Generic;
using System.Linq;
using WellBot.Domain.Chats.Entities;

namespace WellBot.UseCases.Chats
{
    /// <summary>
    /// Service related to random item generation or random item selection.
    /// </summary>
    public class RandomService
    {
        private readonly Random random = new Random();

        /// <summary>
        /// Pick a random element from a collection.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <returns>Random element.</returns>
        public T PickRandom<T>(IEnumerable<T> source)
        {
            int elementIndex = random.Next(source.Count());
            return source.ElementAt(elementIndex);
        }

        /// <summary>
        /// Pick a random element from a list.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <returns>Random element.</returns>
        public T PickRandom<T>(IList<T> source)
        {
            int elementIndex = random.Next(source.Count);
            return source[elementIndex];
        }

        /// <summary>
        /// Get a random value from 0 to <paramref name="max"/>.
        /// </summary>
        /// <param name="max">Exclusive max value.</param>
        /// <returns>Random value.</returns>
        public int GetRandom(int max) => random.Next(max);

        /// <summary>
        /// Pick a random item with a certain weight.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">Item collection.</param>
        /// <returns>Randomly selected item.</returns>
        public T PickWeighted<T>(IReadOnlyCollection<T> items) where T : IWeighted
        {
            // Prepare a weighted list.
            var weightedItems = items.Select(m => (m, GetWeight(m.Weight, items.Count)))
                .ToList();

            var totalWeight = weightedItems.Sum(m => m.Item2);
            var desiredWeight = GetRandom(totalWeight);
            var currentWeightSum = 0;
            foreach (var item in weightedItems)
            {
                var currentWeightRange = currentWeightSum + item.Item2;
                if (currentWeightSum <= desiredWeight && desiredWeight < currentWeightRange)
                {
                    return item.m;
                }
                currentWeightSum = currentWeightRange;
            }

            // Weird, shouldn't happen.
            return weightedItems.Last().m;
        }

        private int GetWeight(MessageWeight weight, int itemsCount) => weight switch
        {
            MessageWeight.Highest => itemsCount * 10,
            MessageWeight.High => itemsCount * 2,
            _ => 1
        };

        /// <summary>
        /// Tells that an entity has a certain weight associated with it.
        /// </summary>
        public interface IWeighted
        {
            /// <summary>
            /// Weight of the item.
            /// </summary>
            MessageWeight Weight { get; }
        }
    }
}
