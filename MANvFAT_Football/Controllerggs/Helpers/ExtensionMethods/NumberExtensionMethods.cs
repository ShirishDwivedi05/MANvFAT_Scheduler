using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Helpers.ExtensionMethods
{
    public static class NumberExtensionMethods
    {

        /// <summary>
        /// Returns the long value that was passed in, or a zero if the value was null
        /// </summary>
        public static long NullLongToZero(this long? input)
        {

            if (input == null)
            {
                input = 0;
            }

            return (long)input;

        }

        /// <summary>
        /// Returns the int value that was passed in, or a zero if the value was null
        /// </summary>
        public static int NullIntToZero(this int? input)
        {

            if (input == null)
            {
                input = 0;
            }

            return (int)input;

        }

        /// <summary>
        /// Returns the decimal value that was passed in, or a zero if the value was null
        /// </summary>
        public static decimal NullDecimalToZero(this decimal? input)
        {

            if (input == null)
            {
                input = 0;
            }

            return (decimal)input;

        }

        /// <summary>
        /// Returns the double value that was passed in, or a zero if the value was null
        /// </summary>
        public static double NullDoubleToZero(this double? input)
        {

            if (input == null)
            {
                input = 0;
            }

            return (double)input;

        }


    }
}