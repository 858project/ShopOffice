using Microsoft.EntityFrameworkCore;

namespace ShopOffice.Database
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="options">Context configuration</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Table_Account_DatabaseModel> Table_Accounts { get; set; } = default!;
        public DbSet<Table_Calendar_DatabaseModel> Table_Calendars { get; set; } = default!;
        public DbSet<Table_Product_DatabaseModel> Table_Products { get; set; } = default!;
        public DbSet<Table_Sale_DatabaseModel> Table_Sales { get; set; } = default!;
        public DbSet<View_Calendar_DatabaseModel> View_Calendars { get; set; } = default!;
        public DbSet<View_DailyReport_DatabaseModel> View_DailyReports { get; set; } = default!;
        public DbSet<View_Sale_DatabaseModel> View_Sales { get; set; } = default!;
    }
}
