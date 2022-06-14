using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;
using ShopOffice.Controls;
using ShopOffice.Database;
using ShopOffice.Enums;

namespace ShopOffice.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="configuration">Configuration for this application</param>
        /// <param name="databaseFactory">Database factory</param>
        public MainWindow(ILoggerFactory loggerFactory, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
        {
            // initialize components
            this.InitializeComponent();

            // set data
            this.DatabaseFactory = databaseFactory;
            this.LoggerFactory = loggerFactory;
            this.Configuration = configuration;
            this.Logger = loggerFactory.CreateLogger<MainWindow>();

            // set data context
            this.DataContext = this;
        }
        #endregion

        #region - Events -
        /// <summary>
        /// Envent for property change
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
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
        /// This value determine whether user is loged
        /// </summary>
        private Boolean IsAccount
        {
            get { return this.Account != null; }
        }
        /// <summary>
        /// Current account
        /// </summary>
        public Table_Account_DatabaseModel? Account
        {
            get
            {
                return this.mAccount;
            }
            private set
            {
                // change property
                this.mAccount = value;

                // notify change
                this.InternalNotifyPropertyChanged();
            }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Current account
        /// </summary>
        private Table_Account_DatabaseModel? mAccount = null;
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method is called by the Set accessor of each property.  
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void InternalNotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// This method invalidates ribbon buttons
        /// </summary>
        private void InternalInvalidateRibbon()
        {
            this.InternalInvalidateRibbon(this.Account != null);
        }
        /// <summary>
        /// This method invalidates ribbon buttons
        /// </summary>
        /// <param name="state">Current state</param>
        private void InternalInvalidateRibbon(Boolean state)
        {
            //this.rrbConnect.IsEnabled = !state;
            //this.rrbDisconnect.IsEnabled = state;
        }
        /// <summary>
        /// This method opens login windows for logging
        /// </summary>
        private void InternalSignIn()
        {
            // create login dialog
            LoginWindow window = new LoginWindow(this.LoggerFactory, this.Configuration, this.DatabaseFactory)
            {
                Owner = this
            };
 
            // show login dialg
            if (window.ShowDialog() == true)
            {
                // save account data
                this.Account = window.Account;

                // invalidate current ribbon
                this.InternalInvalidateRibbon();

                // set sale control
                this.InternalSetControl(ControlTypes.Sale);
            }
        }
        /// <summary>
        /// This function clears user's data 
        /// </summary>
        private void InternalClear()
        {
            this.ccPanel.Content = null;
        }
        /// <summary>
        /// This method opens login windows for logging
        /// </summary>
        private void InternalSignOut()
        {
            // reset account data
            this.Account = null;

            // clear data
            this.InternalClear();

            // invalidate ribbon
            this.InternalInvalidateRibbon();
        }
        /// <summary>
        /// This methods execute login for user
        /// </summary>
        /// <param name="sender">Login button</param>
        /// <param name="e">Arguments</param>
        private void RrbLogin_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSignIn();
        }
        /// <summary>
        /// This methods execute logout for user
        /// </summary>
        /// <param name="sender">Logout button</param>
        /// <param name="e">Arguments</param>
        private void RrbLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSignOut();
        }       
        private Boolean InternalIsControlKey(KeyEventArgs e)
        {
            return e.Key == Key.LeftCtrl ||
                   e.Key == Key.RightCtrl ||
                   Keyboard.IsKeyDown(Key.LeftCtrl) ||
                   Keyboard.IsKeyDown(Key.RightCtrl) ||
                   (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }
        private Boolean InternalIsAltKey(KeyEventArgs e)
        {
            return e.Key == Key.LeftAlt ||
                   e.Key == Key.RightAlt ||
                   Keyboard.IsKeyDown(Key.LeftAlt) ||
                   Keyboard.IsKeyDown(Key.RightAlt) ||
                   (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        }
        /// <summary>
        /// This method executes key from window
        /// </summary>
        /// <param name="sender">Current window</param>
        /// <param name="e">Arguments</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(String.Format("Key down: {0} - Ctrl: {1} - Alt: {2} - State: {3} - IsRepeat: {4}", 
                e.Key, this.InternalIsControlKey(e), this.InternalIsAltKey(e), e.KeyStates, e.IsRepeat));

            // check current user
            if (this.IsAccount)
            {
                // request for sale control
                if (this.InternalIsControlKey(e) && this.InternalIsAltKey(e))
                {
                    // request for daily report control
                    if (e.Key == Key.Y)
                    {
                        Debug.WriteLine("Executing CTRL + ALT + Y...");
                        this.InternalSetControl(ControlTypes.DailyReport);

                        // this key was used
                        e.Handled = true;
                    }
                }
                else if (this.InternalIsControlKey(e))
                {
                    if (e.Key == Key.E)
                    {
                        Debug.WriteLine("Executing CTRL + E...");
                        this.InternalSetControl(ControlTypes.Sale);

                        // this key was used
                        e.Handled = true;
                    }
                    // request for product control
                    else if (e.Key == Key.D)
                    {
                        Debug.WriteLine("Executing CTRL + D...");
                        this.InternalSetControl(ControlTypes.Products);

                        // this key was used
                        e.Handled = true;
                    }
                    // request for calendar control
                    else if (e.Key == Key.C)
                    {
                        Debug.WriteLine("Executing CTRL + C...");
                        this.InternalSetControl(ControlTypes.Calendar);

                        // this key was used
                        e.Handled = true;
                    }
                }
                else if (this.ccPanel.Content is ControlBase control)
                {
                    Debug.WriteLine("Executing key for control...");
                    control.ExecuteKey(e.Key);

                    // this key was used
                    e.Handled = true;
                }
            }
        }
        private void InternalInitializeContextualTabGroup(ControlTypes type)
        {
            if (type == ControlTypes.Products)
            {
                //this.rrctgProductContextualTabGroup.Visibility = Visibility.Visible;
            }
            else
            {
                //this.rrctgProductContextualTabGroup.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// This method sets control to window
        /// </summary>
        /// <param name="type">Control type</param>
        private void InternalSetControl(ControlTypes type)
        {
            // get control
            ControlBase? control = this.InternalGetControl(type);
            if (control != null)
            {
                // set control
                this.ccPanel.Content = control;

                // initialize control
                control.Initialize(TimeSpan.FromMilliseconds(500));

                // initialize contextua tab
                this.InternalInitializeContextualTabGroup(type);
            }
        }
        /// <summary>
        /// This metod gets control accoring to type
        /// </summary>
        /// <param name="type">control type</param>
        /// <returns>Control base</returns>
        private ControlBase? InternalGetControl(ControlTypes type)
        {
            if (this.Account is not null)
            {
                switch (type)
                {
                    case ControlTypes.Sale:
                        return new SaleControl(this.LoggerFactory, this.Account, this.biBusy, this.Configuration, this.DatabaseFactory);
                    case ControlTypes.DailyReport:
                        return new DailyReportControl(this.LoggerFactory, this.Account, this.biBusy, this.Configuration, this.DatabaseFactory);
                    case ControlTypes.Products:
                        return new ProductsControl(this.LoggerFactory, this.Account, this.biBusy, this.Configuration, this.DatabaseFactory);
                    case ControlTypes.Calendar:
                        return new CalendarControl(this.LoggerFactory, this.Account, this.biBusy, this.Configuration, this.DatabaseFactory);
                    default:
                        break;
                }
            }
            return null;
        }
        /// <summary>
        /// This method executes operation after open window
        /// </summary>
        /// <param name="sender">Tis window</param>
        /// <param name="e">Arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.InternalSignIn();
        }
        #endregion

        private void miSignIn_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSignIn();
        }

        private void miSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSignOut();
        }

        private void miAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (this.ccPanel.Content != null)
            {
                var context = this.ccPanel.Content;
                if (context is ProductsControl productControl)
                {
                    productControl.AddProduct();
                }
            }
        }

        private void miProducts_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSetControl(ControlTypes.Products);
        }

        private void miSale_Click(object sender, RoutedEventArgs e)
        {
            this.InternalSetControl(ControlTypes.Sale);
        }




        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }
    }
}
