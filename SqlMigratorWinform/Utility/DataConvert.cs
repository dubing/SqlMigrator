using System;
using System.Globalization;

namespace SqlMigratorWinform.Utility
{
    public static class DataConvert
    {
        public static object ChangeType(object value, Type type, object defaultValue)
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool? ParseBool(string value, bool? defaultValue)
        {
            bool result;
            if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static byte? ParseByte(string value, byte? defaultValue)
        {
            byte result;
            if (!string.IsNullOrEmpty(value) && byte.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static byte[] ParseByteArray(string value, byte[] defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? ParseDateTime(string value, DateTime? defaultValue)
        {
            DateTime result;
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            if (!DateTime.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static decimal? ParseDecimal(string value, decimal? defaultValue)
        {
            decimal result;
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            if (!decimal.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static double? ParseDouble(string value, double? defaultValue)
        {
            double result;
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            if (!double.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static Enum ParseEnum(string value, Enum defaultValue)
        {
            return ParseEnum(value, defaultValue, false);
        }

        public static Enum ParseEnum(string value, Enum defaultValue, bool ignoreCase)
        {
            try
            {
                return (Enum.Parse(defaultValue.GetType(), value, ignoreCase) as Enum);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static Guid? ParseGuid(string value, Guid? defaultValue)
        {
            try
            {
                return new Guid(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? ParseInt(string value, int? defaultValue)
        {
            return ParseInt(value, defaultValue, false);
        }

        public static int? ParseInt(string value, int? defaultValue, bool isHexNumber)
        {
            int result;
            if (!string.IsNullOrEmpty(value) && ((isHexNumber && int.TryParse(value, NumberStyles.HexNumber, new NumberFormatInfo(), out result)) || int.TryParse(value, out result)))
            {
                return result;
            }
            return defaultValue;
        }

        public static long? ParseLong(string value, long? defaultValue)
        {
            return ParseLong(value, defaultValue, false);
        }

        public static long? ParseLong(string value, long? defaultValue, bool isHexNumber)
        {
            long result;
            if (!string.IsNullOrEmpty(value) && ((isHexNumber && long.TryParse(value, NumberStyles.HexNumber, new NumberFormatInfo(), out result)) || long.TryParse(value, out result)))
            {
                return result;
            }
            return defaultValue;
        }

        public static Type ParseType(string vlaue, Type defaultValue)
        {
            return ParseType(vlaue, defaultValue, false);
        }

        public static Type ParseType(string vlaue, Type defaultValue, bool ignoreCase)
        {
            return TypeHelper.TryGetType(vlaue, defaultValue, ignoreCase);
        }

        public static string ToString(object obj)
        {
            return ((obj == null) ? string.Empty : obj.ToString());
        }

        public static string ToString(object obj, string format)
        {
            return ToString(obj, format, null);
        }

        public static string ToString(object obj, string format, IFormatProvider provider)
        {
            var f = obj as IFormattable;
            if (f == null)
            {
                return ToString(obj);
            }
            return f.ToString(format, provider);
        }
    }
}

