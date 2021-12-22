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
            int elementIndex = GetRandom(source.Count());
            return source.ElementAt(elementIndex);
        }

        /// <summary>
        /// Pick a random element from a list.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <returns>Random element.</returns>
        public T PickRandom<T>(IList<T> source)
        {
            int elementIndex = GetRandom(source.Count);
            return source[elementIndex];
        }

        /// <summary>
        /// Get a random value from 0 to <paramref name="max"/>.
        /// </summary>
        /// <param name="max">Exclusive max value.</param>
        /// <returns>Random value.</returns>
        public int GetRandom(int max) => MathNet.Numerics.Distributions.DiscreteUniform.Sample(random, 0, max - 1);

        /// <summary>
        /// Pick a random item with a certain weight.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">Item collection.</param>
        /// <returns>Randomly selected item.</returns>
        public T PickWeightedRaw<T>(IReadOnlyCollection<T> items) where T : IWeightedRaw
        {
            // Prepare a weighted list.
            var weightedItems = items.Select(m => new WeightedItem<T>(m, m.Weight))
                .ToList();
            return PickWeightedInternal(weightedItems);
        }

        /// <summary>
        /// Pick a random item with a certain weight.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">Item collection.</param>
        /// <returns>Randomly selected item.</returns>
        public T PickWeighted<T>(IReadOnlyCollection<T> items) where T : IWeighted
        {
            // Prepare a weighted list.
            var weightedItems = items.Select(m => new WeightedItem<T>(m, GetWeight(m.Weight)))
                .ToList();
            return PickWeightedInternal(weightedItems);
        }

        private T PickWeightedInternal<T>(IReadOnlyCollection<WeightedItem<T>> items)
        {
            var totalWeight = items.Sum(m => m.Weight);
            var desiredWeight = GetRandom(totalWeight);
            var currentWeightSum = 0;
            foreach (var item in items)
            {
                var currentWeightRange = currentWeightSum + item.Weight;
                if (currentWeightSum <= desiredWeight && desiredWeight < currentWeightRange)
                {
                    return item.Item;
                }
                currentWeightSum = currentWeightRange;
            }

            // Weird, shouldn't happen.
            return items.Last().Item;
        }

        private int GetWeight(MessageWeight weight) => weight switch
        {
            MessageWeight.Highest => 100,
            MessageWeight.High => 12,
            _ => 1
        };

        /// <summary>
        /// Tells that an entity has a certain weight associated with it.
        /// </summary>
        public interface IWeightedRaw
        {
            /// <summary>
            /// Weight of the item. The higher is weight the more likely it is for an option to be chosen.
            /// </summary>
            int Weight { get; }
        }

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

        private class WeightedItem<T>
        {
            public WeightedItem(T item, int weight)
            {
                Item = item;
                Weight = weight;
            }

            public T Item { get; init; }

            public int Weight { get; init; }
        }
    }
}
