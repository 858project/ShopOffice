using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ShopOffice
{
    public static class Utility
    {
        /// <summary>
        /// This method computes SHA1 hash from input string
        /// </summary>
        /// <param name="value">Input string to compute</param>
        /// <returns>Computed hash</returns>
        public static String EncryptoSHA1(String value)
        {
            return Utility.EncryptoSHA1(value, Encoding.UTF8);
        }
        /// <summary>
        /// This method computes SHA1 hash from input string
        /// </summary>
        /// <param name="value">Input string to compute</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Computed hash</returns>
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
        /// <summary>
        /// This method return name for calendar
        /// </summary>
        /// <param name="type">Calendar type</param>
        /// <returns>Calendar name</returns>
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
    }
}
