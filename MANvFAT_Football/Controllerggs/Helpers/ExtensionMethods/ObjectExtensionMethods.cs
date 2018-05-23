using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Helpers.ExtensionMethods
{
    public static class ObjectExtensionMethods
    {

        /// <summary>
        /// Returns the string value that was passed in, or an empty string if the value was null
        /// </summary>
        public static string NullObjectToEmptyString(this object input)
        {

            return GeneralHelper.DenullString(input);

        }

        /// <summary>
        /// Returns the bool value that was passed in, or false if the value was null
        /// </summary>
        public static bool NullObjectToFalse(this object input)
        {

            return GeneralHelper.DenullBoolean(input);

        }

    }
}