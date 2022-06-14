using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Product table from database
    /// </summary>
    [Table("v_Sale", Schema = "dbo")]
    public sealed class View_Sale_DatabaseModel
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
        /// Product code id
        /// </summary>
        public Int32 CodeId { get; set; }
        /// <summary>
        /// Product name
        /// </summary>
        public String Name { get; set; } = default!;
        /// <summary>
        /// This value determine whether item was sold
        /// </summary>
        public Boolean Sold { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public Int32 Amount { get; set; }
        /// <summary>
        /// Total price
        /// </summary>
        public Int32 TotalAmount
        {
            get
            {
                return Convert.ToInt32((this.Quantity / 1000.0f) * this.Amount);
            }
        }
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
