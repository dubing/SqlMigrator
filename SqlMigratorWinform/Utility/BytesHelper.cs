using System;
using System.Text;

namespace SqlMigratorWinform.Utility
{
    public static class BytesHelper
    {
        public static bool CompareBytes(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1.Length != bytes2.Length)
            {
                return false;
            }
            return CompareBytes(bytes1, 0, bytes2, 0, bytes1.Length);
        }

        public static bool CompareBytes(byte[] bytes1, int index1, byte[] bytes2, int index2, int count)
        {
            if ((bytes1.Length - index1) < count)
            {
                return false;
            }
            if ((bytes2.Length - index2) < count)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                if (bytes1[i + index1] != bytes2[i + index2])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EndsWith(byte[] bytes1, byte[] bytes2)
        {
            return EndsWith(bytes1, 0, bytes1.Length, bytes2, 0, bytes2.Length);
        }

        public static bool EndsWith(byte[] bytes1, int index1, int count1, byte[] bytes2, int index2, int count2)
        {
            if ((count1 - count2) < 0)
            {
                return false;
            }
            return (FindBytes(bytes1, (index1 + count1) - count2, count2, bytes2, index2, count2) >= 0);
        }

        public static int FindByte(byte[] bytes, byte bt)
        {
            return FindByte(bytes, 0, bytes.Length, bt);
        }

        public static int FindByte(byte[] bytes, int index, int count, byte bt)
        {
            return FindBytes(bytes, index, count, new[] { bt }, 0, 1);
        }

        public static int FindBytes(byte[] bytes1, byte[] bytes2)
        {
            return FindBytes(bytes1, 0, bytes1.Length, bytes2, 0, bytes2.Length);
        }

        public static int FindBytes(byte[] bytes1, int index1, byte[] bytes2)
        {
            return FindBytes(bytes1, index1, bytes1.Length - index1, bytes2, 0, bytes2.Length);
        }

        public static int FindBytes(byte[] bytes1, int index1, int count1, byte[] bytes2, int index2, int count2)
        {
            if ((count1 - count2) >= 0)
            {
                int i = index1;
                int i1 = (count1 + index1) - count2;
                while (i <= i1)
                {
                    bool isFind = true;
                    int j = index2;
                    int j1 = index2 + count2;
                    while (j < j1)
                    {
                        if (bytes1[(i + j) - index2] != bytes2[j])
                        {
                            isFind = false;
                            break;
                        }
                        j++;
                    }
                    if (isFind)
                    {
                        return i;
                    }
                    i++;
                }
            }
            return -1;
        }

        public static byte[] FromBase64String(string s)
        {
            return Convert.FromBase64String(s);
        }

        public static byte[] FromString(string s)
        {
            return FromString(s, Encoding.UTF8);
        }

        public static byte[] FromString(string s, Encoding encoding)
        {
            return encoding.GetBytes(s);
        }

        public static byte[] JoinBytes(byte[] bytes1, byte[] bytes2)
        {
            return JoinBytes(bytes1, 0, bytes1.Length, bytes2, 0, bytes2.Length);
        }

        public static byte[] JoinBytes(byte[] bytes1, int index1, int count1, byte[] bytes2, int index2, int count2)
        {
            byte[] result = new byte[count1 + count2];
            Buffer.BlockCopy(bytes1, index1, result, 0, count1);
            Buffer.BlockCopy(bytes2, index2, result, count1, count2);
            return result;
        }

        public static bool StartsWith(byte[] bytes1, byte[] bytes2)
        {
            return StartsWith(bytes1, 0, bytes2, 0, bytes2.Length);
        }

        public static bool StartsWith(byte[] bytes1, int index1, byte[] bytes2, int index2, int count2)
        {
            if (((bytes1.Length - index1) - count2) < 0)
            {
                return false;
            }
            return (FindBytes(bytes1, index1, count2, bytes2, index2, count2) >= 0);
        }

        public static byte[] SubBytes(byte[] bytes, int index)
        {
            return SubBytes(bytes, index, bytes.Length - index);
        }

        public static byte[] SubBytes(byte[] bytes, int index, int count)
        {
            byte[] result = new byte[count];
            Buffer.BlockCopy(bytes, index, result, 0, count);
            return result;
        }

        public static string ToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToBase64String(byte[] bytes, int index, int count)
        {
            return Convert.ToBase64String(bytes, index, count);
        }

        public static string ToString(byte[] bytes)
        {
            return ToString(bytes, 0, bytes.Length, Encoding.UTF8);
        }

        public static string ToString(byte[] bytes, int index, int count, Encoding encoding)
        {
            return encoding.GetString(bytes, index, count);
        }
    }
}

