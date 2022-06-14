using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using ShopOffice.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopOffice.Configuration;
using ShopOffice.Notifications;

namespace ShopOffice.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="configuration">Configuration for this application</param>
        /// <param name="databaseFactory">Database factory</param>
        public LoginWindow(ILoggerFactory loggerFactory, IOptionsMonitor<ConfigurationModel> configuration, IDbContextFactory<DatabaseContext> databaseFactory)
        {
            // initialize components
            this.InitializeComponent();

            // set data
            this.DatabaseFactory = databaseFactory;
            this.LoggerFactory = loggerFactory;
            this.Configuration = configuration;
            this.Logger = loggerFactory.CreateLogger<LoginWindow>();

            // set data context
            this.DataContext = this;
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
        /// Model for this window
        /// </summary>
        public Table_Account_DatabaseModel? Account
        {
            get { return this.cbAccount.SelectedItem as Table_Account_DatabaseModel; }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This methods initializes data for this window
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">Arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // access to database
            using (var database = this.DatabaseFactory.CreateDbContext())
            {
                var items = database.Table_Accounts.ToList();
                if (items != null && items.Count > 0x00)
                {
                    foreach (var item in items)
                    {
                        this.cbAccount.Items.Add(item);
                    }
                    this.cbAccount.SelectedIndex = 0x00;
                }
            }

            // set focut to login
            FocusManager.SetFocusedElement(this, this.tbLogin);
        }
        /// <summary>
        /// This method clears comboBox error
        /// </summary>
        /// <param name="sender">ComboBox</param>
        /// <param name="e">Arguments</param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
        /// <summary>
        /// This method clears textBox error
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">Arguments</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
        /// <summary>
        /// This function validates form and its data
        /// </summary>
        /// <returns>TRUE | FALSE</returns>
        private Boolean InternalValidateData()
        {
            // validate account
            if (this.cbAccount.SelectedItem == null && !(this.cbAccount.SelectedItem is Table_Account_DatabaseModel))
            {
                this.cbAccount.BorderBrush = Brushes.Red;
                return false;
            }

            // validate login
            if (String.IsNullOrWhiteSpace(this.tbLogin.Text))
            {
                this.tbLogin.BorderBrush = Brushes.Red;
                return false;
            }

            // validate password
            if (String.IsNullOrWhiteSpace(this.pbPassword.Password))
            {
                this.pbPassword.BorderBrush = Brushes.Red;
                return false;
            }

            // data is valid
            return true;
        }
        /// <summary>
        /// This function validates login and password
        /// </summary>
        /// <returns>TRUE | FALSE</returns>
        private Boolean InternalValidateLogin()
        {
            // get current account
            Table_Account_DatabaseModel? account = this.Account;
            if (account == null)
            {
                return false;
            }

            // get login
            String login = this.tbLogin.Text.Trim();
            if (String.Compare(login, account.Login, false) != 0x00)
            {
                return false;
            }

            // get password
            String password = this.pbPassword.Password.Trim();
            password = Utility.EncryptoSHA1(password);
            if (String.Compare(password, account.Password, false) != 0x00)
            {
                return false;
            }

            // login is correct
            return true;
        }
        /// <summary>
        /// This method executes this window
        /// </summary>
        private void InternalExecute()
        {
            // check data
            if (this.InternalValidateData())
            {
                // check login
                if (!this.InternalValidateLogin())
                {
                    // show message for user
                    NotificationHelper.ShowWarning(Properties.Resources.str_0012);
                    return;
                }

                // end this window
                this.DialogResult = true;
            }
        }
        /// <summary>
        /// This method validates data and check login
        /// </summary>
        /// <param name="sender">Buttin Ok</param>
        /// <param name="e">Arguments</param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.InternalExecute();
        }
        /// <summary>
        /// This method cancels windows without result
        /// </summary>
        /// <param name="sender">Buttin Cancel</param>
        /// <param name="e">Arguments</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// This method executes login after enter key
        /// </summary>
        /// <param name="sender">Current window</param>
        /// <param name="e">Arguments</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.InternalExecute();
            }
        }
        #endregion

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
