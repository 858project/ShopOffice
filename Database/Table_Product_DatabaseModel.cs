using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Product table from database
    /// </summary>
    [Table("tbl_Product", Schema = "dbo")]
    public sealed class Table_Product_DatabaseModel
    {
        /// <summary>
        /// Product id
        /// </summary>
        [Key]
        public Int32 Id { get; set; }
        /// <summary>
        /// Account id
        /// </summary>
        public Int32 AccountId { get; set; }
        /// <summary>
        /// This value determine availability
        /// </summary>
        public Boolean Availability { get; set; }
        /// <summary>
        /// Product code id
        /// </summary>
        public Int32 CodeId { get; set; }
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
        /// <summary>
        /// Minimum number
        /// </summary>
        public Int32 MinQuantity { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// Create date
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Update date
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
