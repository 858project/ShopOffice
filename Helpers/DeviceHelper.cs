using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elcom.ElcommLib;
using ShopOffice.Database;
using ShopOffice.Models;

namespace ShopOffice.Helpers
{
    public static class DeviceHelper
    {
        /// <summary>
        /// This enum defines type of payment
        /// </summary>
        public enum PaymentTypes
        {
            Cash,
            Credit
        }

        public static void Execute(String? portName, List<View_Sale_DatabaseModel> collection, PaymentTypes type)
        {
            using (CCommLib lib = new CCommLib())
            {
                // initialize
                lib.Initialize("SK");

                // set active cash register
                eCashRegisterUID activeCashRegister = eCashRegisterUID.Euro50TE;
                eGRetVal eGRetVal = lib.SetActiveCashRegister(activeCashRegister);
                eErrLevel errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[1] = {0} - {1}", errorLevel, errorMessage));
                }

                // set port
                String command = String.Format("{0}:38400,n,8,1", portName);
                lib.SetConfigValue("port", command);
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[2] = {0} - {1}", errorLevel, errorMessage));
                }

                // connect
                eGRetVal = lib.Connect(null, null);
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[3] = {0} - {1}", errorLevel, errorMessage));
                }

                // open receipt
                eGRetVal = lib.OpenReceipt("registration", "sale");
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[4] = {0} - {1}", errorLevel, errorMessage));
                }
             
                // otvorenie uctenky
                eGRetVal = lib.ReceiptCommand("BR", "");
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[5] = {0} - {1}", errorLevel, errorMessage));
                }
                
                // loop all items
                foreach (View_Sale_DatabaseModel item in collection)
                {
                    // sale item
                    // SI - predaj tovarovej položky (Sell Item) z databázy počítača 
                    // "SI", "item\t10.00\t1\t1\t*\t1"
                    // DB - predaj tovarovej položky/vratného obalu (DataBase) z databázy pokladnice 
                    // "DB", "ID\t\tQUANTITY\t\t"
                    // TEST: 211, 220, 18
                    command = String.Format("{0}\t\t{1:0.000}\t*\t1.000", item.CodeId, (item.Quantity / 1000.0f)).Replace(",", ".");
                    eGRetVal = lib.ReceiptCommand("DB", command);
                    errorLevel = lib.GetErrorLevel(eGRetVal);
                    if (errorLevel != eErrLevel.AllOk)
                    {
                        string errorMessage;
                        lib.GetErrorText(eGRetVal, out errorMessage);
                        throw new Exception(String.Format("Sending data to the cash register failed. Error[6] = {0} - {1}", errorLevel, errorMessage));
                    }
                }
               
                // payment
                Double totalAmount = collection.Sum(x => x.TotalAmount / 100.0d);
                command = String.Format("{0:0.00}\t{1}", totalAmount, DeviceHelper.InternalGetPaymentType(type)).Replace(",", ".");
                eGRetVal = lib.ReceiptCommand("PV", command);
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[7] = {0} - {1}. Command: \"{2}\"", errorLevel, errorMessage, command));
                }

                // close receipt
                eGRetVal = lib.CloseReceipt();
                errorLevel = lib.GetErrorLevel(eGRetVal);
                if (errorLevel != eErrLevel.AllOk)
                {
                    string errorMessage;
                    lib.GetErrorText(eGRetVal, out errorMessage);
                    throw new Exception(String.Format("Sending data to the cash register failed. Error[8] = {0} - {1}", errorLevel, errorMessage));
                }
 
                // disconnect
                lib.Disconnect();
            }
        }

        private static String InternalGetPaymentType(PaymentTypes type)
        {
            switch (type)
            {
                case PaymentTypes.Credit:
                    return "credit";
                case PaymentTypes.Cash:
                default:
                    return "cash";
            }
        }
    }
}
