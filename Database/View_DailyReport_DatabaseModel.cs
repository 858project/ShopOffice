using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Product table from database
    /// </summary>
    [Table("v_DailyReport", Schema = "dbo")]
    public sealed class View_DailyReport_DatabaseModel
    {
        /// <summary>
        /// Account id
        /// </summary>
        [Key]
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
        /// Update date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Update date
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
