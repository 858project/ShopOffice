using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOffice.Database
{
    /// <summary>
    /// Account model from database
    /// </summary>
    [Table("tbl_Account", Schema = "dbo")]
    public sealed class Table_Account_DatabaseModel
    {
        #region - Properties -
        /// <summary>
        /// Account id
        /// </summary>
        [Key]
        public Int32 Id { get; set; }
        /// <summary>
        /// User first name
        /// </summary>
        public String FirstName { get; set; } = default!;
        /// <summary>
        /// User last name
        /// </summary>
        public String LastName { get; set; } = default!;
        /// <summary>
        /// User login
        /// </summary>
        public String Login { get; set; } = default!;
        /// <summary>
        /// User password
        /// </summary>
        public String Password { get; set; } = default!;
        /// <summary>
        /// Last change date
        /// </summary>
        public DateTime LastChangePassword { get; set; }
        /// <summary>
        /// Create date
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Create date
        /// </summary>
        public DateTime ChangeTime { get; set; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", this.FirstName, this.LastName);
        }
        #endregion
    }
}
