namespace WellBot.DomainServices.Store
{
    /// <summary>
    /// Product SKU validator.
    /// </summary>
    public class ProductSkuValidator
    {
        /// <summary>
        /// Simple SKU validator.
        /// </summary>
        /// <param name="sku">SKU.</param>
        /// <returns>True if SKU is valid, false otherwise.</returns>
        public bool IsValid(string sku) => !string.IsNullOrEmpty(sku) && sku.StartsWith("SK");
    }
}
