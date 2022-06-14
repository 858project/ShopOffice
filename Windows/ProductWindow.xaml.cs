using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopOffice.Database;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace ShopOffice.Windows
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this control
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="product">Product for this window</param>
        /// <param name="databaseFactory">Database factory</param>
        public ProductWindow(ILoggerFactory loggerFactory, Table_Product_DatabaseModel product, IDbContextFactory<DatabaseContext> databaseFactory)
        {
            // check product
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            // initialize window
            InitializeComponent();

            // set data
            this.DatabaseFactory = databaseFactory;
            this.LoggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<LoginWindow>();

            // set product
            this.DataContext = product;
        }
        #endregion

        #region - Properties -
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
        /// Product from window
        /// </summary>
        public Table_Product_DatabaseModel? Product
        {
            get
            {
                return this.DataContext as Table_Product_DatabaseModel;
            }
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method executes this window
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get current model
                Table_Product_DatabaseModel? product = this.DataContext as Table_Product_DatabaseModel;

                // validate dialog
                if (product != null && this.InternalValidate(product))
                {
                    // get value from ADD
                    Nullable<Double> mValue = this.dupQuantityAdd.Value;
                    if (mValue.HasValue && mValue.Value > 0x00)
                    {
                        product.Quantity += Convert.ToInt32(mValue * 1000.0f);
                    }

                    // end this dialo
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
        /// <summary>
        /// This method cancels this window
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // end this dialo
                this.DialogResult = false;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
        /// <summary>
        /// This method validates TransactionRequest dialog and data
        /// </summary>
        /// <returns>True | False</returns>
        private Boolean InternalValidate(Table_Product_DatabaseModel product)
        {
            try
            {
                //pomocna premenna
                Boolean fleg = true;

                // validate product name
                if (!product.Name.Validate(0x03, 0x32))
                {
                    fleg = false;
                    this.tbName.BorderBrush = Brushes.Red;
                }

                // check code
                if (product.CodeId < 0x00)
                {
                    fleg = false;
                    this.iupCodeId.BorderBrush = Brushes.Red;
                }

                // check amount
                if (product.Amount == 0x00)
                {
                    fleg = false;
                    this.dupAmount.BorderBrush = Brushes.Red;
                }

                //vratime stav chyby
                return fleg;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
        /*
        /// <summary>
        /// This function initilize TransactionRequest dialog
        /// </summary>
        private void InternalInitialize()
        {
            // initialize card type
            this.cbCardType.Items.Add(CardTypes.CreditOrDebit);
            this.cbCardType.Items.Add(CardTypes.Voucher);

            // set card type
            this.cbCardType.SelectedIndex = 0x00;

            // set transaction type
            this.cbTransactionType.SelectedIndex = 0x00;
        }
        /// <summary>
        /// This function sets state for TransactionRequest dialog
        /// </summary>
        /// <param name="state">State for items</param>
        private void InternalInvalidateState(Boolean state)
        {
            this.tbtApprovalCode.IsReadOnly = state;
            this.tbtApprovalCode.IsEnabled = !state;
            this.tbSerialNumber.IsReadOnly = state;
            this.tbSerialNumber.IsEnabled = !state;
            this.tbAmount.IsReadOnly = state;
            this.tbAmount.IsEnabled = !state;
            this.tbTip.IsReadOnly = state;
            this.tbTip.IsEnabled = !state;
            this.tbUserCode.IsReadOnly = state;
            this.tbUserCode.IsEnabled = !state;
        }
        /// <summary>
        /// This function change CardType for TransactionRequest dialog
        /// </summary>
        /// <param name="sender">Sender for this event</param>
        /// <param name="e">Arguments</param>
        private void cbCardType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // get card type from comboBox
                CardTypes cardType = (CardTypes)comboBox.SelectedItem;

                // clear transaction type
                this.cbTransactionType.Items.Clear();

                // check card type
                if (cardType != CardTypes.Unknown)
                {
                    // add transaction type
                    this.cbTransactionType.Items.Add(TransactionTypes.Unknown);
                    this.cbTransactionType.Items.Add(TransactionTypes.Sale);
                    this.cbTransactionType.Items.Add(TransactionTypes.SaleWithTip);
                    if (cardType != CardTypes.Voucher)
                    {
                        this.cbTransactionType.Items.Add(TransactionTypes.Refund);
                    }
                    this.cbTransactionType.Items.Add(TransactionTypes.Void);

                    // select first item
                    this.cbTransactionType.SelectedIndex = 0x00;
                }
            }
        }
        /// <summary>
        /// This function change TransactionType for TransactionRequest dialog
        /// </summary>
        /// <param name="sender">Sender for this event</param>
        /// <param name="e">Arguments</param>
        private void cbTransactionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    // get transaction type from comboBox
                    TransactionTypes transactionTypes = (TransactionTypes)comboBox.SelectedItem;

                    // change state for dialog
                    this.InternalInvalidateState(transactionTypes == TransactionTypes.Unknown);

                    // check void operation
                    if (transactionTypes == TransactionTypes.Void)
                    {
                        this.rdAmout.Height = new GridLength(0);
                        this.rdApprovalCode.Height = new GridLength(0, GridUnitType.Auto);
                    }
                    else
                    {
                        this.rdAmout.Height = new GridLength(0, GridUnitType.Auto);
                        this.rdApprovalCode.Height = new GridLength(0);
                    }

                    // check tip item
                    if (transactionTypes == TransactionTypes.SaleWithTip)
                    {
                        this.rdTip.Height = new GridLength(0, GridUnitType.Auto);
                    }
                    else
                    {
                        this.rdTip.Height = new GridLength(0);
                    }
                }
                else
                {
                    this.InternalInvalidateState(true);
                }
            }
        }
        */
        /// <summary>
        /// This method change validation state in text box
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //overime ci odosielatelom eventu je text box
                if (sender is TextBox textBox)
                {
                    textBox.ClearValue(TextBox.BorderBrushProperty);
                }
               
            }
            catch (Exception ex)
            {
                // trace
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        private void DupAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                //overime ci odosielatelom eventu je text box
                if (sender is DoubleUpDown doubleUpDown)
                {
                    doubleUpDown.ClearValue(TextBox.BorderBrushProperty);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
 
        private void DupCodeId_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                //overime ci odosielatelom eventu je text box
                if (sender is IntegerUpDown IntegerUpDown)
                {
                    IntegerUpDown.ClearValue(TextBox.BorderBrushProperty);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
        #endregion

        #region - Window Chrome Methods -
        /// <summary>
        /// This methods check whether command can be executed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        /// <summary>
        /// This method executes close windos command
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
        #endregion
    }
}
