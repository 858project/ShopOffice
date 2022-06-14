using System;

namespace ShopOffice.Models
{
    /// <summary>
    /// Product table from database
    /// </summary>
    public sealed class SaleInputPopupModel
    {
        /// <summary>
        /// Product name
        /// </summary>
        public String Name { get; set; } = default!;
        /// <summary>
        /// Quantity
        /// </summary>
        public Int32 Quantity { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public Int32 Amount { get; set; }
    }
}
