using System;
using System.Collections.Generic;
using System.Linq;

namespace WellBot.UseCases.Common.Collections
{
    /// <summary>
    /// Collection that allows to store items that expire after certain time.
    /// </summary>
    /// <typeparam name="T">Type of stored items.</typeparam>
    public class ExpirableCollection<T>
    {
        private readonly List<ExpirableItem<T>> list = new();

        /// <summary>
        /// Add a new expirable item to collection or update expiration on existing item.
        /// </summary>
        /// <param name="item">Item to prolongate the expiration for.</param>
        /// <param name="expirationUtc">UTC date when the item should be expired.</param>
        public void AddOrUpdate(T item, DateTime expirationUtc)
        {
            Cleanup();
            var expirableItem = list.FirstOrDefault(i => i.Item.Equals(item));
            if (expirableItem != null)
            {
                expirableItem.ExpirationUtc = expirationUtc;
                return;
            }

            list.Add(new ExpirableItem<T>
            {
                Item = item,
                ExpirationUtc = expirationUtc
            });
        }

        /// <summary>
        /// Check if specified item exists in collection and is not expired yet.
        /// </summary>
        /// <param name="item">Item to verify.</param>
        /// <param name="expiresIn">Duration until the item expiration.</param>
        /// <returns>True if item is in collection.</returns>
        public bool Contains(T item, out TimeSpan expiresIn)
        {
            Cleanup();
            var expirableItem = list.FirstOrDefault(i => i.Item.Equals(item));
            if (expirableItem == null)
            {
                expiresIn = TimeSpan.Zero;
                return false;
            }

            expiresIn = expirableItem.ExpirationUtc - DateTime.UtcNow;
            return true;
        }

        private void Cleanup()
        {
            var currentTime = DateTime.UtcNow;
            list.RemoveAll(item => item.ExpirationUtc <= currentTime);
        }

        private class ExpirableItem<TItem>
        {
            public TItem Item { get; init; }

            public DateTime ExpirationUtc { get; set; }
        }
    }
}
