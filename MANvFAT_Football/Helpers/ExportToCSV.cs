using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MANvFAT_Football.Helpers
{
    public static class ExportToCSV
    {
        public static string ToCsvHeader(this object obj)
        {
            Type type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.DeclaredOnly |
                                           BindingFlags.Public |
                                           BindingFlags.Instance);

            string result = string.Empty;
            Array.ForEach(properties, prop =>
            {
                result += prop.Name + ",";
            });

            return (!string.IsNullOrEmpty(result) ? result.Substring(0, result.Length - 1) : result);
        }

        public static string ToCsvRow(this object obj)
        {
            Type type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.DeclaredOnly |
                                           BindingFlags.Public |
                                           BindingFlags.Instance);

            string result = string.Empty;
            Array.ForEach(properties, prop =>
            {
                var value = prop.GetValue(obj, null);
                var propertyType = prop.PropertyType.FullName;
                if (propertyType == "System.String")
                {
                    // wrap value incase of commas
                    value = "\"" + value + "\"";
                }

                result += value + ",";
            });

            return (!string.IsNullOrEmpty(result) ? result.Substring(0, result.Length - 1) : result);
        }
    }
}