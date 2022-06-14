using ShopOffice.Database;
using ShopOffice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShopOffice
{
    public static class Utility
    {
                /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value)
        {
            return Utility.EncryptoSHA1(value, Encoding.UTF8);
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <param name="encoding">Encoding v akom chceme kodovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value, Encoding encoding)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                using (var provider = SHA1.Create())
                {
                    var valueData = encoding.GetBytes(value);
                    var hashData = provider.ComputeHash(valueData);
                    var data = BitConverter.ToString(hashData).Replace("-", "");
                    return data.ToLower();
                }
            }
            return String.Empty;
        }
        public static String? GetNameForCalendarType(Int16 type)
        {
            switch (type)
            {
                case 0x01:
                    return "meniny";
                case 0x02:
                    return "pamätné dni";
                case 0x03:
                    return "medzinárodný deň";
                case 0x04:
                    return "deň pracovného pokoja";
                case 0x05:
                    return "štátný sviatok";
                default:
                    return null;
            }

        }
        public static String? GetConfiguration(String key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            if (appSettings != null)
            {
                if (appSettings.Count > 0x00)
                {
                    return ConfigurationManager.AppSettings[key];
                }  
            }
            return null;
        }
        /// <summary>
        /// This method checks string and its length
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="minLength">Minimalna dlzka ktoru ma mat string, pricom nemoze byt zlozeny len z white space znakov</param>
        /// <param name="maxLength">Maximalna dlzka ktory moze mt string</param>
        /// <returns>True = string ma minimalnu dlzku, inak false</returns>
        public static Boolean Validate(this string value, int minLength, int maxLength)
        {
            return !String.IsNullOrWhiteSpace(value) && value.Length >= minLength && value.Length <= maxLength;
        }
        public static Table_Product_DatabaseModel Clone(Table_Product_DatabaseModel model)
        {
            Table_Product_DatabaseModel product = new Table_Product_DatabaseModel();
            Utility.Copy(model, product);
            return product;
        }
        /// <summary>
        /// This method copies product model to other product model
        /// </summary>
        /// <param name="source">Source model</param>
        /// <param name="destination">Destination model</param>
        public static void Copy(Table_Product_DatabaseModel source, Table_Product_DatabaseModel destination)
        {
            destination.Id = source.Id;
            destination.AccountId = source.AccountId;
            destination.Availability = source.Availability;
            destination.CodeId = source.CodeId;
            destination.Name = source.Name;
            destination.Quantity = source.Quantity;
            destination.Amount = source.Amount;
            destination.MinQuantity = source.MinQuantity;
            destination.Description = source.Description;
            destination.CreateDate = source.CreateDate;
            destination.UpdateDate = source.UpdateDate;
        }
    }
}
