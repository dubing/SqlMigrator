using System.Collections.Generic;

namespace SqlMigratorWinform.Utility
{
    public static class Arithmetic
    {
        public static string[] GetAllInlineItems(string str, char begin, char end)
        {
            var list = new List<string>();
            int ith = 0;
            for (string item = GetInlineItem(str, ith, begin, end); item != null; item = GetInlineItem(str, ith, begin, end))
            {
                list.Add(item);
                ith++;
            }
            return list.ToArray();
        }

        public static string GetInlineItem(string str, int nth, char begin, char end)
        {
            int ith = 0;
            int idx = 0;
            while ((idx >= 0) && (ith <= nth))
            {
                idx = str.IndexOf(begin, idx);
                if (idx == -1)
                {
                    return null;
                }
                ith++;
                idx++;
            }
            int start = idx;
            int eth = 0;
            var anyChars = new[] { begin, end };
            while ((idx >= 0) && (eth >= 0))
            {
                idx = str.IndexOfAny(anyChars, idx);
                if (idx == -1)
                {
                    return null;
                }
                if (str[idx] == begin)
                {
                    eth++;
                }
                else
                {
                    eth--;
                }
                idx++;
            }
            return str.Substring(start, (idx - start) - 1);
        }
    }
}

