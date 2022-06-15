using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ShopOffice.Models;
using System.Linq;
using System.Windows.Controls;
using System.Threading.Tasks;
using ShopOffice.Helpers;
using Xceed.Wpf.Toolkit;
using System.Windows.Threading;
using ShopOffice.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;
using ShopOffice.Notifications;

namespace ShopOffice.Controls
{
    /// <summary>
    /// Interaction logic for SaleControl.xaml
    /// </summary>
    public partial class SaleControl : ControlBase
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="account">Account data</param>
        /// <param name="busyIndicator">Busy indicator</param>
        /// <param name="databaseFactory">Database factory</param>
        public SaleControl(ILoggerFactory loggerFactory, Table_Account_DatabaseModel account, BusyIndicator busyIndicator, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
            : base(loggerFactory, account, busyIndicator, configuration, databaseFactory)
        {
            this.InitializeComponent();
        }
        #endregion

        #region - Destructors -
        /// <summary>
        /// Deinitialize this class
        /// </summary>
        ~SaleControl()
        {
            // check timer
            if (mClockDispatcherTimer != null)
            {
                this.mClockDispatcherTimer.Stop();
                this.mClockDispatcherTimer = null;
            }
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

        #region - Variables -
        /// <summary>
        /// timer for input popup
        /// </summary>
        private DispatcherTimer? mInputPopupTimer = null;
        private Int32 mCalendarIndex = 0x00;
        private Nullable<DateTime> mCalendarDate = null;
        private List<Table_Calendar_DatabaseModel>? mCalendarItems = null;
        private DispatcherTimer? mClockDispatcherTimer = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method executes key from parent window
        /// </summary>
        /// <param name="key">Current key</param>
        public override void ExecuteKey(Key key)
        {
            // check Escape
            if (key == Key.Escape)
            {
                // invalidate data grid
                this.InternalInvalidateDataGrid(DataGridTypes.Sale);

                // set focus for input box
                this.InternalSetDefaultFocus();
            }
        }
        #endregion

        #region - Private Methods -
        private void InternalInvalidateCalendar(Table_Calendar_DatabaseModel model)
        {
            // get calendar type name
            var name = Utility.GetNameForCalendarType(model.Type);
            if (!String.IsNullOrWhiteSpace(name))
            {
                // show information
                this.tbCalendar.Text = String.Format("{0}{1}{2}", name, Environment.NewLine, model.Text);
            }
        }
        private void InternalClockDispatcherTimer_Tick(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                // get current date
                DateTime date = DateTime.Now;

                // set new time
                //this.gClock.PrimaryScale.Value = date.Hour + (date.Minute / 60.0d);
                //this.gClock.SecondaryScales[0].Value = date.Minute;
                //this.gClock.SecondaryScales[1].Value = date.Second;

                // get current date
                date = date.Date;

                // check date
                if (!this.mCalendarDate.HasValue || date != this.mCalendarDate)
                {
                    // set current date
                    this.mCalendarDate = date;

                    // load new data
                    this.InternalLoadCalendarData();
                }

                if (this.mCalendarItems != null)
                {
                    // check calendar
                    if (this.mCalendarIndex % 0x05 == 0x00)
                    {
                        // get current index
                        Int32 index = this.mCalendarIndex / 0x05;

                        // check collection
                        if (this.mCalendarItems.Count <= index)
                        {
                            // reset value
                            index = this.mCalendarIndex = 0x00;
                        }

                        // get calendar item
                        Table_Calendar_DatabaseModel model = this.mCalendarItems[index];

                        // show information
                        this.InternalInvalidateCalendar(model);
                    }

                    // next calendar value
                    this.mCalendarIndex += 0x01;
                }
            });
        }
        private void InternalSetClockMode(Boolean mode)
        {
            this.acControl.Opacity = mode ? 0.85 : 0.17;
        }
        //private void InternalInitializeCalendar()
        //{
        //    // set current date
        //    this.mCalendarDate = DateTime.Now.Date;

        //    // load data for calendar
        //    this.InternalLoadCalendarData();
        //}
        private void InternalLoadCalendarData()
        {
            try
            {
                // check date
                if (this.mCalendarDate.HasValue)
                {
                    // access to database
                    using var database = this.DatabaseFactory.CreateDbContext();

                    // get date
                    DateTime date = this.mCalendarDate.Value;

                    // select data from database
                    var items = database.Table_Calendars.Where(x =>
                                                               (!x.Year.HasValue || x.Year.Value == date.Year) &&
                                                               x.Month == date.Month &&
                                                               x.Day == date.Day)
                                                        .OrderBy(x => x.Type)
                                                        .ToList();

                    // set data
                    this.mCalendarItems = items;
                }
            }
            catch (Exception ex)
            {
                // trace error to file
                this.Logger?.LogError(ex, ex.Message);
            }
        }
        private void InternalInitializeClock()
        {
            /*
            //declare some variables
            var ClockColor = new SolidColorBrush(Color.FromRgb(95, 95, 95));
            var ClockArcColor = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            var ClockFontSize = 30;

            var DialColorHours = Color.FromRgb(180, 180, 180);
            var DialColorMinutes = Color.FromRgb(180, 180, 180);
       
            var LabelColorHours = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            var LabelColorMinutes = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            var LabelColorSeconds = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            var FontFamily = new FontFamily("Arial");

            this.gClock.FontSize = ClockFontSize;
            this.gClock.Stroke = ClockArcColor;
            this.gClock.StrokeThickness = 5;

            //create the hour scales
            var Hours = new Scale()
            {
                AngleBegin = 60,
                AngleEnd = 60,
                ArcStrokeThickness = 2,
                ArcStroke = ClockArcColor,
                DialColor1 = DialColorHours,
                DialLengthFactor = 0.6,
                DialShape = DialShape.DefaultNeedle,
                Label = new TextBlock(),
                MajorTickCount = 12,
                MinorTickCount = 0,
                RangeBegin = 1,
                RangeEnd = 13,
                ValueIndicatorDistance = -99999,
                MajorTicks = new MajorTicksLine()
                {
                    FontFamily = FontFamily,
                    FontWeight = FontWeights.Bold,
                    LabelBrush = LabelColorHours,
                    LabelOffset = -12,
                    OffsetA = -5,
                    TickStroke = LabelColorHours,
                }
            };

            //create the minute scales
            var Minutes = new Scale()
            {
                AngleBegin = 90,
                AngleEnd = 90,
                DialColor1 = DialColorMinutes,
                DialLengthFactor = 0.8,
                DialShape = DialShape.DefaultNeedle,
                MajorTickCount = 60,
                MinorTickCount = 0,
                RangeBegin = 0,
                RangeEnd = 60,
                Theme = ScaleThemeEnum.None,
                MajorTicks = new MajorTicksLine()
                {
                    LabelBrush = LabelColorMinutes,
                    LabelsEnabled = false,
                    OffsetA = -4,
                    OffsetB = -2,
                    TickStroke = LabelColorMinutes,
                    TickThickness = 2
                }
            };

            //create the second scales
            var Seconds = new Scale()
            {
                AngleBegin = 90,
                AngleEnd = 90,
                DialLengthFactor = 0.8,
                DialShape = DialShape.Line,
                MajorTickCount = 60,
                MinorTickCount = 0,
                RangeBegin = 0,
                RangeEnd = 60,
                Theme = ScaleThemeEnum.None,
                MajorTicks = new MajorTicksLine()
                {
                    LabelsEnabled = false,
                    OffsetA = -4,
                    OffsetB = -2,
                    TickStroke = LabelColorSeconds,
                    TickThickness = 2
                }
            };

            //add the scales to the clock
            this.gClock.PrimaryScale = Hours;
            this.gClock.SecondaryScales.Add(Minutes);
            this.gClock.SecondaryScales.Add(Seconds);

            // get current date
            DateTime date = DateTime.Now;

            //set the current time
            this.gClock.PrimaryScale.Value = date.Hour + (date.Minute / 60.0d);
            this.gClock.SecondaryScales[0].Value = date.Minute;
            this.gClock.SecondaryScales[1].Value = date.Second;
            this.gClock.Height = Double.NaN;
            this.gClock.Width = Double.NaN;
            this.gClock.Visibility = Visibility.Visible;
            */

            // initialize thimer for clock
            mClockDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            mClockDispatcherTimer.Tick += this.InternalClockDispatcherTimer_Tick;
            mClockDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            mClockDispatcherTimer.Start();
        }
        /// <summary>
        /// This method initialize current control
        /// </summary>
        protected override void InternalInitialize()
        {
            // initialize clock
            this.InternalInitializeClock();

            // initialize calendar
            this.InternalLoadCalendarData();

            // load data for this control
            this.InternalInvalidateData();

            // set default focus
            this.InternalSetDefaultFocus();

            // invalidate controls
            this.InternalInvalidateDataGrid(DataGridTypes.Sale);
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
                Keyboard.Focus(this.tbInput);

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
            Task.Delay(span).ContinueWith((_) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    // set defualt foucs
                    this.InternalSetDefaultFocus();
                }));
            });
        }
        /// <summary>
        /// This method executes operation in this control
        /// </summary>
        private void InternalExecute()
        {
            // get text
            String command = this.tbInput.Text.Trim();

            // check command
            if (!String.IsNullOrWhiteSpace(command))
            {
                // busy state
                this.InternalSetBusyState(true);

                try
                {
                    this.InternalExecute(command);
                }
                catch (Exception ex)
                {
                    // trace
                    this.Logger?.LogError(ex, ex.Message);

                    // show error for user
                    NotificationHelper.ShowError(ex.Message);
                }

                // busy state
                this.InternalSetBusyState(false);
            }
        }
        /// <summary>
        /// This method executes operation in this control
        /// </summary>
        /// <param name="data">Data to parse</param>
        private void InternalExecute(String data)
        {
            // check input data
            if (!String.IsNullOrWhiteSpace(data))
            {
                // parse data
                SaleInputModel? inputMmodel = SaleInputModel.Parse(data);
                if (inputMmodel != null)
                {
                    // execute operation
                    this.InternalExecute(inputMmodel);

                    // reload data
                    this.InternalInvalidateData();

                    // show data grid
                    this.InternalInvalidateDataGrid(DataGridTypes.Sale);
                }
                else
                {
                    // search operation
                    this.InternalSearch(data);

                    // show data grid
                    this.InternalInvalidateDataGrid(DataGridTypes.Product);
                }
            }
        }
        /// <summary>
        /// This function makes sale in database
        /// </summary>
        /// <param name="database">Database access</param>
        /// <returns>True | False</returns>
        private Boolean InternalSale(DatabaseContext database)
        {
            // transaction for all data
            using var transaction = database.Database.BeginTransaction();

            try
            {
                // select data from database
                var items = database.Table_Sales.Where(x =>
                                                       !x.Sold &&
                                                       x.AccountId == this.Account.Id)
                                                .ToList();

                // checkdata
                if (items?.Count > 0x00)
                {
                    // lopp all items
                    foreach (Table_Sale_DatabaseModel item in items)
                    {
                        // update sale item
                        item.UpdateDate = DateTime.Now;
                        item.Sold = true;

                        // update model
                        database.Table_Sales.Update(item);

                        // get product from database
                        var product = database.Table_Products.FirstOrDefault(x =>
                                                                             x.Id == item.ProductId &&
                                                                             x.AccountId == this.Account.Id);

                        // check product
                        if (product != null)
                        {
                            // update product
                            product.Quantity = Math.Max(0x00, product.Quantity - item.Quantity);
                            product.UpdateDate = DateTime.Now;

                            // update model
                            database.Table_Products.Update(product);
                        }

                        // save change to database
                        database.SaveChanges();
                    }

                    // commit data to database
                    transaction.Commit();

                    // success
                    return true;
                }

                // no data to save
                return false;
            }
            catch (Exception ex)
            {
                // rollback
                transaction.Rollback();

                // trace
                this.Logger?.LogError(ex, ex.Message);

                // error
                return false;
            }
        }
        /// <summary>
        /// This method executes sale operation
        /// </summary>
        /// <returns>True | False</returns>
        private Boolean InternalExecuteSale()
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // make sale in database
            return this.InternalSale(database);
        }
        ///// <summary>
        ///// This method executes sale operation with card
        ///// </summary>
        ///// <param name="model">Model with data</param>
        ///// <returns>True | False</returns>
        //private Boolean InternalExecuteSaleWithCard(SaleInputModel model)
        //{
        //    // access to database
        //    using (var database = this.DatabaseFactory.CreateDbContext())
        //    {
        //        // load data
        //        var items = database.View_Sales.Where(x =>
        //                                              !x.Sold &&
        //                                              x.AccountId == this.Account.Id)
        //                                       .ToList();

        //        // check count
        //        if (items != null && items.Count > 0x00)
        //        {
        //            try
        //            {
        //                // execute sale with cash register
        //                DeviceHelper.Execute(Utility.GetConfiguration("DeviceComPort"), items, DeviceHelper.PaymentTypes.Credit);

        //                // make sale in database
        //                return this.InternalSale(database);
        //            }
        //            catch (Exception ex)
        //            {
        //                // trace error
        //                this.Logger?.LogError(ex, ex.Message);

        //                // show message for user
        //                NotificationHelper.ShowError(String.Format(Properties.Resources.str_0020, ex.Message));
        //                return false;
        //            }
        //        }

        //        // no data to execute
        //        return true;
        //    }
        //}
        /// <summary>
        /// This method executes sale operation with cash
        /// </summary>
        /// <param name="paymentTypes">Payment type</param>
        /// <returns>True | False</returns>
        private Boolean InternalExecuteSale(DeviceHelper.PaymentTypes paymentTypes)
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // load data
            var items = database.View_Sales.Where(x =>
                                                  !x.Sold &&
                                                  x.AccountId == this.Account.Id)
                                           .ToList();

            // check count
            if (items?.Count > 0x00)
            {
                try
                {
                    // execute sale with cash register
                    DeviceHelper.Execute(this.Configuration.CurrentValue.DeviceComPort, items, paymentTypes);

                    // make sale in database
                    return this.InternalSale(database);
                }
                catch (Exception ex)
                {
                    // trace error
                    this.Logger?.LogError(ex, ex.Message);

                    // show message for user
                    NotificationHelper.ShowError(String.Format(Properties.Resources.str_0020, ex.Message));
                    return false;
                }
            }

            // no data to execute
            return true;
        }
        /// <summary>
        /// This method removes product from sale
        /// </summary>
        /// <param name="model">Model with data</param>
        private void InternalExecuteRemoveProduct(SaleInputModel model)
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // get product from database
            var product = database.Table_Products.FirstOrDefault(x =>
                                                                 x.CodeId == model.Code &&
                                                                 x.AccountId == this.Account.Id);

            // check product
            if (product == null)
            {
                // show error
                NotificationHelper.ShowWarning(String.Format(Properties.Resources.str_0013, model.Code));
                return;
            }

            // get sold item
            var sale = database.Table_Sales.FirstOrDefault(x =>
                                                           !x.Sold &&
                                                           x.ProductId == product.Id &&
                                                           x.AccountId == this.Account.Id);

            // check sale
            if (sale == null)
            {
                // show error
                NotificationHelper.ShowWarning(String.Format(Properties.Resources.str_0031, model.Code));
                return;
            }

            // check count
            if (sale.Quantity <= model.Count)
            {
                // delete object
                database.Table_Sales.Remove(sale);
            }
            else
            {
                // update model
                sale.UpdateDate = DateTime.Now;
                sale.Quantity -= Convert.ToInt32(model.Count);

                // update model to database
                database.Table_Sales.Update(sale);
            }

            // save change to database
            database.SaveChanges();
        }
        /// <summary>
        /// This function adds product to sale
        /// </summary>
        /// <param name="model">Model with data</param>
        private void InternalExecuteAddProduct(SaleInputModel model)
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // get product from database
            var product = database.Table_Products.FirstOrDefault(x =>
                                                                 x.CodeId == model.Code &&
                                                                 x.AccountId == this.Account.Id);

            // check product
            if (product == null)
            {
                // show error
                NotificationHelper.ShowWarning(String.Format(Properties.Resources.str_0013, model.Code));
                return;
            }

            // get sold item
            var sale = database.Table_Sales.FirstOrDefault(x =>
                                                           !x.Sold &&
                                                           x.ProductId == product.Id &&
                                                           x.AccountId == this.Account.Id);

            // check count
            Int32 quantity = product.Quantity - ((sale?.Quantity) ?? 0x00);
            if (Convert.ToUInt32(quantity) < model.Count)
            {
                // show error
                NotificationHelper.ShowWarning(String.Format(Properties.Resources.str_0014, quantity));
                return;
            }

            // check sale
            if (sale == null)
            {
                // create new model
                sale = new Table_Sale_DatabaseModel()
                {
                    AccountId = this.Account.Id,
                    Amount = product.Amount,
                    ProductId = product.Id,
                    Quantity = Convert.ToInt32(model.Count),
                    Sold = false,
                    Status = false,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                // insert model to database
                database.Table_Sales.Add(sale);
            }
            else
            {
                // update model
                sale.UpdateDate = DateTime.Now;
                sale.Quantity += Convert.ToInt32(model.Count);

                // update model to database
                database.Table_Sales.Update(sale);
            }

            // save change
            database.SaveChanges();
        }
        /// <summary>
        /// This method executes operation in this control
        /// </summary>
        /// <param name="model">Model with data</param>
        private void InternalExecute(SaleInputModel model)
        {
            // check operation
            switch (model.Operation)
            {
                case SaleInputModel.OperationTypes.Clear:
                {
                    // execute clear operation
                    this.InternalExecuteClear();
                    break;
                }
                case SaleInputModel.OperationTypes.AddProduct:
                {
                    // execute add-product operation
                    this.InternalExecuteAddProduct(model);
                    break;
                }
                case SaleInputModel.OperationTypes.RemoveProduct:
                {
                    // execute remove-product operation
                    this.InternalExecuteRemoveProduct(model);
                    break;
                }
                case SaleInputModel.OperationTypes.Sale:
                {
                    // execute sale
                    if (this.InternalExecuteSale())
                    {
                        // invalidate data
                        this.InternalInvalidateData();
                    }
                    break;
                }
                case SaleInputModel.OperationTypes.SaleWithCash:
                {
                    // execute sale with cash
                    if (this.InternalExecuteSale(DeviceHelper.PaymentTypes.Cash))
                    {
                        // invalidate data
                        this.InternalInvalidateData();
                    }
                    break;
                }
                case SaleInputModel.OperationTypes.SaleWithCard:
                {
                    // execute sale with cash
                    if (this.InternalExecuteSale(DeviceHelper.PaymentTypes.Credit))
                    {
                        // invalidate data
                        this.InternalInvalidateData();
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
        /// <summary>
        /// This method invalidates data
        /// </summary>
        private void InternalInvalidateData()
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // select data
            var items = database.View_Sales.Where(x =>
                                                  !x.Sold &&
                                                  x.AccountId == this.Account.Id)
                                           .ToList();

            // set data
            this.dgSaleItems.ItemsSource = items;

            // update price
            float totalCount = items?.Count > 0x00 ? items.Sum(x => (x.Quantity / 1000.0f) * x.Amount) : 0x00;
            this.tbTotalAmount.Text = String.Format("{0:0.00} €", totalCount / 100.0f);
        }
        /// <summary>
        /// This method show one datagrid
        /// </summary>
        /// <param name="type"></param>
        private void InternalInvalidateDataGrid(DataGridTypes type)
        {
            if (type == DataGridTypes.Sale)
            {
                var data = this.dgSaleItems.ItemsSource;
                if (data == null || data is not List<View_Sale_DatabaseModel> || ((List<View_Sale_DatabaseModel>)data).Count == 0x00)
                {
                    this.InternalSetClockMode(true);
                    this.tbCalendar.Visibility = Visibility.Visible;
                    this.dgProductItems.Visibility = Visibility.Hidden;
                    this.dgSaleItems.Visibility = Visibility.Hidden;
                    this.tbTotalAmount.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.InternalSetClockMode(false);
                    this.tbCalendar.Visibility = Visibility.Hidden;
                    this.dgProductItems.Visibility = Visibility.Collapsed;
                    this.dgSaleItems.Visibility = Visibility.Visible;
                    this.tbTotalAmount.Visibility = Visibility.Visible;
                }
            }
            else if (type == DataGridTypes.Product)
            {
                this.InternalSetClockMode(false);
                this.tbCalendar.Visibility = Visibility.Hidden;
                this.dgSaleItems.Visibility = Visibility.Collapsed;
                this.dgProductItems.Visibility = Visibility.Visible;
                this.tbTotalAmount.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// This function search data
        /// </summary>
        /// <param name="data">Data to search</param>
        private void InternalSearch(String data)
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // select data
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
        /// <summary>
        /// This function executes clear operation
        /// </summary>
        private void InternalExecuteClear()
        {
            // access to database
            using var database = this.DatabaseFactory.CreateDbContext();

            // select all items
            var items = database.Table_Sales.Where(x =>
                                                   !x.Sold &&
                                                   x.AccountId == this.Account.Id)
                                            .ToList();

            // loop all items
            foreach (var item in items)
            {
                // remove items from database
                database.Table_Sales.Remove(item);
            }

            // save change in database
            database.SaveChanges();
        }
        /// <summary>
        /// This method shows information about product in popup
        /// </summary>
        /// <param name="model">Product information | NULL</param>
        private void InternalShowInputPopup(Table_Product_DatabaseModel? model)
        {
            // check timer for popup
            if (mInputPopupTimer == null)
            {
                mInputPopupTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromSeconds(4)
                };
                mInputPopupTimer.Tick += (object? sender, EventArgs e) =>
                {
                    if (sender != null)
                    {
                        ((DispatcherTimer)sender).IsEnabled = false;
                    }

                    if (this.pInput.IsOpen)
                    {
                        this.pInput.IsOpen = false;
                    }
                };
            }

            // stop timer
            mInputPopupTimer.IsEnabled = false;

            // check product
            if (model != null)
            {
                // set data 
                this.pInput.DataContext = new SaleInputPopupModel()
                {
                    Amount = model.Amount,
                    Name = model.Name,
                    Quantity = model.Quantity,
                };

                // show popup
                this.pInput.IsOpen = true;

                // start timer
                mInputPopupTimer.IsEnabled = true;
            }
            else
            {
                // hide popup
                this.pInput.IsOpen = false;
            }
        }
        /// <summary>
        /// This method detect change in input and read product for this command
        /// </summary>
        /// <param name="sender">Input text box</param>
        /// <param name="e">arguments</param>
        private void TbInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // get text box
            if (sender is TextBox textBox)
            {
                // get text
                String command = textBox.Text;

                // check command
                if (!String.IsNullOrWhiteSpace(command))
                {
                    // parse data
                    SaleInputModel? inputMmodel = SaleInputModel.Parse(command);
                    if (inputMmodel != null && inputMmodel.Code != 0x00)
                    {
                        // execute task in background
                        Task.Run(() =>
                        {
                            // access to database
                            using var database = this.DatabaseFactory.CreateDbContext();

                            // get product from database
                            var product = database.Table_Products.FirstOrDefault(x =>
                                                                                 x.CodeId == inputMmodel.Code &&
                                                                                 x.AccountId == this.Account.Id);

                            // check product
                            if (product != null)
                            {
                                // main thread
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                        // show product
                                        this.InternalShowInputPopup(product);
                                }));
                            }
                        });
                    }
                }
                else
                {
                    // show product
                    this.InternalShowInputPopup(null);
                }
            }
        }
        /// <summary>
        /// This method executes internal operation in this control
        /// </summary>
        /// <param name="sender">Input text box</param>
        /// <param name="e">Arguments</param>
        private void TbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.Handled)
            {
                // this event was handled
                e.Handled = true;

                // execute this control
                this.InternalExecute();

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
            if (element != null && element is FrameworkElement element1 && element1.Parent is DataGridCell)
            {
                var grid = sender as DataGrid;
                if (grid?.SelectedItems?.Count == 1)
                {
                    if (grid.SelectedItem is Table_Product_DatabaseModel product)
                    {
                        // create input
                        SaleInputModel input = new(SaleInputModel.OperationTypes.AddProduct)
                        {
                            Code = Convert.ToUInt32(product.CodeId),
                            Count = 0x01
                        };

                        // execute this operation
                        this.InternalExecute(input);

                        // reload data
                        this.InternalInvalidateData();

                        // invalidate data grid
                        this.InternalInvalidateDataGrid(DataGridTypes.Sale);

                        // set default focus
                        this.InternalSetDefaultFocus(TimeSpan.FromSeconds(1));
                    }
                }
            }
        }
        #endregion
    }
}
