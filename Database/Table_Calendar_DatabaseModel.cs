using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Product table from database
    /// </summary>
    [Table("tbl_Calendar", Schema = "dbo")]
    public sealed class Table_Calendar_DatabaseModel
    {
        /// <summary>
        /// Calendar id
        /// </summary>
        [Key]
        public Int32 Id { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public Int16 Type { get; set; }
        /// <summary>
        /// Day
        /// </summary>
        public Int32 Day { get; set; }
        /// <summary>
        /// Month
        /// </summary>
        public Int32 Month { get; set; }
        /// <summary>
        /// Year
        /// </summary>
        public Nullable<Int32> Year { get; set; }
        /// <summary>
        /// Text
        /// </summary>
        public String Text { get; set; } = default!;
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
