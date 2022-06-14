using System;
using System.Collections.Generic;
using System.Windows.Input;
using ShopOffice.Models;
using System.Linq;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
using ShopOffice.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;

namespace ShopOffice.Controls
{
    /// <summary>
    /// Interaction logic for SaleControl.xaml
    /// </summary>
    public partial class DailyReportControl : ControlBase
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="account">Account data</param>
        /// <param name="busyIndicator">Busy indicator</param>
        /// <param name="configuration">Configuration for this application</param>
        /// <param name="databaseFactory">Database factory</param>
        public DailyReportControl(ILoggerFactory loggerFactory, Table_Account_DatabaseModel account, BusyIndicator busyIndicator, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
            : base(loggerFactory, account, busyIndicator, configuration, databaseFactory)
        {
            InitializeComponent();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Current date for this control
        /// </summary>
        private DateTime mDate = DateTime.Now.Date;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method executes key from parent window
        /// </summary>
        /// <param name="key">Current key</param>
        public override void ExecuteKey(Key key)
        {
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method initialize current control
        /// </summary>
        protected override void InternalInitialize()
        {
            // set busy state
            this.InternalSetBusyState(true);

            // load data for this control
            this.InternalInvalidateData();

            // set busy state
            this.InternalSetBusyState(false);
        }
        /// <summary>
        /// This method invalidates data
        /// </summary>
        private void InternalInvalidateData()
        {
            // access to database
            using (var database = this.DatabaseFactory.CreateDbContext())
            {
                // get items
                var items = database.View_DailyReports.Where(x =>
                                                             x.AccountId == this.Account.Id &&
                                                             x.Date.Date == mDate.Date)
                                                      .ToList();

                // set items
                this.dgSaleItems.ItemsSource = items;

                // update price
                float totalCount = items != null && items.Count > 0x00 ? items.Sum(x => ((x.Quantity / 1000.0f) * x.Amount)) : 0x00;
                this.tbTotalAmount.Text = String.Format("{0:0.00} €", (totalCount / 100.0f));
            }
        }
        #endregion
    }
}
