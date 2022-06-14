using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Product table from database
    /// </summary>
    [Table("tbl_Sale", Schema = "dbo")]
    public sealed class Table_Sale_DatabaseModel
    {
        /// <summary>
        /// Sale id
        /// </summary>
        [Key]
        public Int32 Id { get; set; }
        /// <summary>
        /// Account id
        /// </summary>
        public Int32 AccountId { get; set; }
        /// <summary>
        /// Product id
        /// </summary>
        public Int32 ProductId { get; set; }
        /// <summary>
        /// This value determine whether item was sold
        /// </summary>
        public Boolean Sold { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public Int32 Amount { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public Int32 Quantity { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public Boolean Status { get; set; }
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
