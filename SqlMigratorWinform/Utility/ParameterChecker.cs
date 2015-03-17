using System;
using System.Collections;

namespace SqlMigratorWinform.Utility
{
    public static class ParameterChecker
    {
        public static void CheckNull(string method, string paraName, object paraValue)
        {
            if (paraValue == null)
            {
                throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNull, method));
            }
        }

        public static void CheckNullOrEmpty(string method, string paraName, Array paraValue)
        {
            CheckNullOrEmpty(method, paraName, paraValue, false);
        }

        public static void CheckNullOrEmpty(string method, string paraName, ArrayList paraValue)
        {
            CheckNullOrEmpty(method, paraName, paraValue, false);
        }

        public static void CheckNullOrEmpty(string method, string paraName, string paraValue)
        {
            if (string.IsNullOrEmpty(paraValue))
            {
                throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNullOrEmpty, method));
            }
        }

        public static void CheckNullOrEmpty(string method, string paraName, Array paraValue, bool deep)
        {
            if ((paraValue == null) || (paraValue.Length == 0))
            {
                throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNullOrEmpty, method));
            }
            if (deep)
            {
                int i = 0;
                int j = paraValue.Length;
                while (i < j)
                {
                    if (paraValue.GetValue(i) == null)
                    {
                        throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNull, method + ParameterCheckerMessage.Items));
                    }
                    i++;
                }
            }
        }

        public static void CheckNullOrEmpty(string method, string paraName, ArrayList paraValue, bool deep)
        {
            if ((paraValue == null) || (paraValue.Count == 0))
            {
                throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNullOrEmpty, method));
            }
            if (deep)
            {
                int i = 0;
                int j = paraValue.Count;
                while (i < j)
                {
                    if (paraValue[i] == null)
                    {
                        throw new ArgumentNullException(paraName, string.Format(ParameterCheckerMessage.CheckNull, method + ParameterCheckerMessage.Items));
                    }
                    i++;
                }
            }
        }

        public static void CheckRange(string method, string paraName, decimal paraValue, decimal minValue)
        {
            CheckRange(method, paraName, paraValue, minValue, 79228162514264337593543950335M);
        }

        public static void CheckRange(string method, string paraName, int paraValue, int minValue)
        {
            CheckRange(method, paraName, paraValue, minValue, 0x7fffffff);
        }

        public static void CheckRange(string method, string paraName, decimal paraValue, decimal minValue, decimal maxValue)
        {
            if ((paraValue < minValue) || (paraValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(paraName, paraValue, string.Format(ParameterCheckerMessage.CheckRange, method));
            }
        }

        public static void CheckRange(string method, string paraName, int paraValue, int minValue, int maxValue)
        {
            if ((paraValue < minValue) || (paraValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(paraName, paraValue, string.Format(ParameterCheckerMessage.CheckRange, method));
            }
        }
    }
}
