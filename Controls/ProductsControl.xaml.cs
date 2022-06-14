using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Threading;
using ShopOffice.Windows;
using System.Linq;
using ShopOffice.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;
using Notification.Wpf;
using ShopOffice.Notifications;

namespace ShopOffice.Controls
{
    /// <summary>
    /// Interaction logic for SaleControl.xaml
    /// </summary>
    public partial class ProductsControl : ControlBase
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="account">Account data</param>
        /// <param name="busyIndicator">Busy indicator</param>
        /// <param name="databaseFactory">Database factory</param>
        public ProductsControl(ILoggerFactory loggerFactory, Table_Account_DatabaseModel account, BusyIndicator busyIndicator, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
            : base(loggerFactory, account, busyIndicator, configuration, databaseFactory)
        {
            InitializeComponent();
        }
        #endregion

        #region - Enum -
        /// <summary>
        /// This enum defines grid type
        /// </summary>
        public enum DataGridTypes
        {
            Product,
            Sale
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This function executes function for creating new product
        /// </summary>
        public void AddProduct()
        {
            this.InternalAddProduct();
        }
        /// <summary>
        /// This method executes key from parent window
        /// </summary>
        /// <param name="key">Current key</param>
        public override void ExecuteKey(Key key)
        {
            // check Escape
            if (key == Key.Escape)
            {
                // set focus for input box
                this.InternalSetDefaultFocus();
            }
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function executes function for creating new product
        /// </summary>
        private void InternalAddProduct()
        {
            // create new product
            Table_Product_DatabaseModel product = new Table_Product_DatabaseModel();

            // open product window
            ProductWindow window = new ProductWindow(this.LoggerFactory, product, this.DatabaseFactory)
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true && window.Product != null)
            {
                this.InternalInsertOrUpdateProduct( window.Product);
            }
        }
        /// <summary>
        /// This method initialize current control
        /// </summary>
        protected override void InternalInitialize()
        {
            // load data for this control
            this.InternalInvalidateData();

            // set default focus
            this.InternalSetDefaultFocus();
        }
        /// <summary>
        /// This method sets default focus
        /// </summary>
        private void InternalSetDefaultFocus()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // set focut to input
                FocusManager.SetFocusedElement(this, this.tbInput);

                // set focus
                this.tbInput.Focus();

                // select all data
                this.tbInput.SelectAll();

            }), DispatcherPriority.Render);
        }
        /// <summary>
        /// This method sets default focus
        /// </summary>
        /// <param name="span">Delay</param>
        private void InternalSetDefaultFocus(TimeSpan span)
        {
            Task.Delay(span).ContinueWith((task) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    // set defualt foucs
                    this.InternalSetDefaultFocus();
                }));
            });
        }
        /// <summary>
        /// This method invalidates data
        /// </summary>
        private void InternalInvalidateData()
        {
            // set busy state
            this.InternalSetBusyState(true);

            // load data
            this.InternalInvalidateData(this.tbInput.Text);

            // set busy state
            this.InternalSetBusyState(false);
        }
        /// <summary>
        /// This method invalidates data
        /// </summary>
        /// <param name="data">Search input</param>
        private void InternalInvalidateData(String data)
        {
            // access to database
            using (var database = this.DatabaseFactory.CreateDbContext())
            {
                // select account data
                var itemsQuery = database.Table_Products.Where(x =>
                                                               x.AccountId == this.Account.Id);

                // search cause
                if (!String.IsNullOrWhiteSpace(data))
                {
                    // like operator
                    itemsQuery = itemsQuery.Where(x =>
                                                  EF.Functions.Like(x.Name, $"%{data}%") ||
                                                  EF.Functions.Like(x.CodeId.ToString(), $"%{data}%"));
                }

                // get data
                var items = itemsQuery.ToList();

                // set data
                this.dgProductItems.ItemsSource = items;
            }
        }
        /// <summary>
        /// This method executes internal operation in this control
        /// </summary>
        /// <param name="sender">Input text box</param>
        /// <param name="e">Arguments</param>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            // check key
            if (e.Key == Key.Enter && !e.Handled)
            {
                // hide tooltip
                ToolTipService.SetIsEnabled(this.tbInput, true);

                // this event was handled
                e.Handled = true;

                // execute this control
                this.InternalInvalidateData();

                // set default focus
                this.InternalSetDefaultFocus();
            }
        }
        /// <summary>
        /// This method returns new unique number for product
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        private Int32 InternalGetNewCodeId(DatabaseContext database)
        {
            var result = database.Table_Products.Where(x =>
                                                       x.AccountId == this.Account.Id)
                                                .Max(x => x.CodeId);

            return ((Int32)result) + 0x01;
        }
        /// <summary>
        /// This method inserts or update product to database
        /// </summary>
        /// <param name="product">Product to insert or update</param>
        private void InternalInsertOrUpdateProduct(Table_Product_DatabaseModel product)
        {
            // access to database
            using (var database = this.DatabaseFactory.CreateDbContext())
            {
                try
                {
                    // get db product
                    var productInDatabase = product.Id != 0x00 ? database.Table_Products.FirstOrDefault(x =>
                                                                                   x.Id == product.Id &&
                                                                                   x.AccountId == this.Account.Id)
                                                               : null;

                    // is new product?
                    if (productInDatabase != null)
                    {
                        // copy data
                        productInDatabase.Availability = product.Availability;
                        productInDatabase.CodeId = product.CodeId;
                        productInDatabase.Name = product.Name;
                        productInDatabase.Quantity = product.Quantity;
                        productInDatabase.Amount = product.Amount;
                        productInDatabase.MinQuantity = product.MinQuantity;
                        productInDatabase.Description = product.Description;
                        productInDatabase.UpdateDate = DateTime.Now;

                        // update product
                        database.Table_Products.Update(productInDatabase);
                    }
                    else
                    {
                        // create new product
                        productInDatabase = new Table_Product_DatabaseModel()
                        {
                            CodeId = this.InternalGetNewCodeId(database),
                            AccountId = this.Account.Id,
                            Availability = product.Availability,
                            Name = product.Name,
                            Quantity = product.Quantity,
                            Amount = product.Amount,
                            MinQuantity = product.MinQuantity,
                            Description = product.Description,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                        };

                        // insert data
                        database.Table_Products.Add(productInDatabase);
                    }

                    // save change in database
                    database.SaveChanges();

                    // execute this control
                    this.InternalInvalidateData();
                }
                catch (Exception ex)
                {
                    // trace error
                    this.Logger?.LogError(ex, ex.Message);

                    // show message
                    NotificationHelper.ShowError(ex.Message);
                }

                // set default focus
                this.InternalSetDefaultFocus();
            }
        }
        /// <summary>
        /// This method select product from data grid
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Arguments</param>
        private void DgProductItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                if (((FrameworkElement)element).Parent is DataGridCell)
                {
                    var grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        Table_Product_DatabaseModel? product = grid.SelectedItem as Table_Product_DatabaseModel;
                        if (product != null)
                        {
                            // open product window
                            ProductWindow window = new ProductWindow(this.LoggerFactory, product, this.DatabaseFactory)
                            {
                                Owner = Window.GetWindow(this)
                            };
                            if (window.ShowDialog() == true && window.Product != null)
                            {
                                this.InternalInsertOrUpdateProduct(window.Product);
                            }

                            /*
                            // create input
                            SaleInputModel input = new SaleInputModel(SaleInputModel.OperationTypes.AddProduct)
                            {
                                Code = Convert.ToUInt32(product.CodeId),
                                Count = 0x01
                            };

                            // execute this operation
                            this.InternalExecute(input);

                            // reload data
                            this.InternalInvalidateData();

                            // set default focus
                            this.InternalSetDefaultFocus(TimeSpan.FromSeconds(1));
                            */
                        }
                    }
                }
            }
        }
        #endregion

        private void tbInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
