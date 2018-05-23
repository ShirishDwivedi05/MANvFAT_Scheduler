using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MANvFAT_Football.Helpers.ExtensionMethods
{
    public static class DateExtensionMethods
    {

        /// <summary>
        ///for nullable datetime variables.
        /// If the date is non null, convert it to string with "dd/MM/yyyy" format.  If it's null, return an empty string
        /// </summary>
        public static string ConvertDateToUkDateString(this DateTime? input)
        {

            if (input != null)
            {
                return (Convert.ToDateTime(input)).ToString("dd/MM/yyyy");
            }
            else
            {
                return "";
            }
        }
    }
}