using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;
using ShopOffice.Database;
using ShopOffice.Models;
using Xceed.Wpf.Toolkit;

namespace ShopOffice.Controls
{
    /// <summary>
    /// This control defines rules for child control
    /// </summary>
    public abstract class ControlBase : UserControl
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
        protected ControlBase(ILoggerFactory loggerFactory, Table_Account_DatabaseModel account, BusyIndicator busyIndicator, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
        {
            this.Account = account;
            this.BusyIndicator = busyIndicator;
            this.DatabaseFactory = databaseFactory;
            this.LoggerFactory = loggerFactory;
            this.Configuration = configuration;
            this.Logger = loggerFactory.CreateLogger<ControlBase>();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Configuration for this application
        /// </summary>
        public IOptionsMonitor<ConfigurationModel> Configuration
        {
            get;
            set;
        }
        /// <summary>
        /// Database factory
        /// </summary>
        public IDbContextFactory<DatabaseContext> DatabaseFactory
        {
            get;
            set;
        }
        /// <summary>
        /// Logger factory
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }
        /// <summary>
        /// Logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            get;
            set;
        }
        /// <summary>
        /// Busy indicator for this control
        /// </summary>
        private BusyIndicator BusyIndicator
        {
            get;
            set;
        }
        /// <summary>
        /// Current account data
        /// </summary>
        protected Table_Account_DatabaseModel Account
        {
            get;
            private set;
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This function sets busy state
        /// </summary>
        /// <param name="state">New state</param>
        public void InternalSetBusyState(Boolean state)
        {
            this.BusyIndicator.IsBusy = state;
        }
        /// <summary>
        /// This method initializes control and its part
        /// </summary>
        /// <param name="span"></param>
        public void Initialize(Nullable<TimeSpan> span)
        {
            if (span.HasValue)
            {
                Task.Delay(span.Value).ContinueWith((task) =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        // set defualt foucs
                        this.InternalInitialize();
                    }));
                });
            }
        }
        /// <summary>
        /// This method executes key from parent window
        /// </summary>
        /// <param name="key">Current key</param>
        public abstract void ExecuteKey(Key key);
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method initializes control and its parts
        /// </summary>
        protected abstract void InternalInitialize();
        #endregion
    }
}
