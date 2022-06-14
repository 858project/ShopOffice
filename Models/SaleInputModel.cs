using System;
using System.Text.RegularExpressions;

namespace ShopOffice.Models
{
    /// <summary>
    /// Model represents data for sale operation
    /// </summary>
    public sealed class SaleInputModel
    {
        #region - Static Constructor -
        /// <summary>
        /// Initialize static class
        /// </summary>
        static SaleInputModel()
        {
            //inicializujeme regex na parsovanie
            SaleInputModel.mReges = new Regex(@"^((?<minus>-)((?<count>\d+)|(?<count>(\d+(\.|,)\d+)))\*(?<codeId>\d+))$|" +
                                              @"^((?<minus>-)(?<codeId>\d+))$|" +
                                              @"^\+?(((?<count>\d+)|(?<count>(\d+(\.|,)\d+)))\*(?<codeId>\d+))$|" +
                                              @"^\+?((?<codeId>\d+))$");
        }
        #endregion

        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="operation">Operation type</param>
        public SaleInputModel(OperationTypes operation)
        {
            this.Operation = operation;
        }
        #endregion

        #region - Enum -
        /// <summary>
        /// This enum defines operations which are supported
        /// </summary>
        public enum OperationTypes
        {
            Clear,
            Sale,
            SaleWithCash,
            SaleWithCard,
            AddProduct,
            RemoveProduct
        }
        #endregion

        #region - Static Variable -
        /// <summary>
        /// Regex for parsing input data
        /// </summary>
        private static Regex mReges = default!;
        #endregion

        #region - Properties -
        /// <summary>
        /// Count from data
        /// </summary>
        public UInt32 Count
        {
            get; set;
        }
        /// <summary>
        /// Code from data
        /// </summary>
        public UInt32 Code
        {
            get; set;
        }
        /// <summary>
        /// Operation type
        /// </summary>
        public OperationTypes Operation
        {
            get; private set;
        }
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// This methods parses input data to model
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <returns>Result | null</returns>
        public static SaleInputModel? Parse(String data)
        {
            // check data to parse
            if (!String.IsNullOrWhiteSpace(data))
            {
                // check operation
                if (String.Compare(data, "--", 0x00) == 0x00)
                {
                    return new SaleInputModel(OperationTypes.Clear);
                }
                else if (String.Compare(data, "+", 0x00) == 0x00)
                {
                    return new SaleInputModel(OperationTypes.Sale);
                }
                else if (String.Compare(data, "++", 0x00) == 0x00)
                {
                    return new SaleInputModel(OperationTypes.SaleWithCash);
                }
                else if (String.Compare(data, "+++", 0x00) == 0x00)
                {
                    return new SaleInputModel(OperationTypes.SaleWithCard);
                }
                else
                {
                    return SaleInputModel.InternalParse(data);
                }
            }

            // invalid data
            return null;
        }
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// This methods parses input data to model
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <returns>Result | null</returns>
        private static SaleInputModel? InternalParse(String data)
        {
            // parse data
            Match match = SaleInputModel.mReges.Match(data);
            if (match.Success)
            {
                // check minus
                String minusValue = match.Groups["minus"].Value;

                // get count
                String countValue = match.Groups["count"].Value;

                // get codeId
                String codeIdValue = match.Groups["codeId"].Value;

                // get operation
                OperationTypes operation = String.Compare(minusValue, "-") != 0x00 ? OperationTypes.AddProduct : OperationTypes.RemoveProduct;
                SaleInputModel model = new SaleInputModel(operation);

                // parse count
                Double count = 0x00;
                if (Double.TryParse(countValue, out count))
                {
                    model.Count = Convert.ToUInt32(count * 1000);
                }
                else
                {
                    model.Count = 0x03E8;
                }

                // parse code id
                UInt32 codeId = 0x00;
                if (UInt32.TryParse(codeIdValue, out codeId))
                {
                    model.Code = codeId;
                }

                // return model with data
                return model;
            }

            // invalid data
            return null;
        }
        #endregion
    }
}
